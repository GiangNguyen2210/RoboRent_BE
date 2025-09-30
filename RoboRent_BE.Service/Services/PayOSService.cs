using RoboRent_BE.Model.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using RoboRent_BE.Model.DTOS;
using RoboRent_BE.Model.DTOS.Account;
using RoboRent_BE.Repository.Interface;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interface;

namespace RoboRent_BE.Service.Services;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;
    private readonly ILogger<PayOSService> _logger;
    private readonly IPaymentTransactionRepository _paymentTransactionRepository;
    private readonly IAccountRepository _accountRepository;
    private const long InitialOrderCode = 20; // Giá trị khởi đầu

    public PayOSService(
        IConfiguration config, 
        ILogger<PayOSService> logger, 
        IPaymentTransactionRepository paymentTransactionRepository,
        IAccountRepository accountRepository)
    {
        _logger = logger;
        _paymentTransactionRepository = paymentTransactionRepository;
        _accountRepository = accountRepository;

        var payosCredPath = Environment.GetEnvironmentVariable("PAYOS_CREDENTIALS");
        if (string.IsNullOrEmpty(payosCredPath))
        {
            throw new ArgumentNullException("PAYOS_CREDENTIALS", "Environment variable PAYOS_CREDENTIALS is not set.");
        }

        string payosCredJson;
        if (File.Exists(payosCredPath))
        {
            payosCredJson = File.ReadAllText(payosCredPath);
            _logger.LogInformation($"Loaded PayOS credentials from file: {payosCredPath}");
        }
        else
        {
            payosCredJson = payosCredPath;
            _logger.LogInformation("Loaded PayOS credentials from environment variable as JSON string.");
        }

        var payosCredentials = JsonSerializer.Deserialize<PayOSCredentials>(payosCredJson);
        if (payosCredentials == null)
        {
            throw new Exception("Failed to parse PayOS credentials.");
        }

        _payOS = new PayOS(payosCredentials.ClientId, payosCredentials.ApiKey, payosCredentials.ChecksumKey);

        _returnUrl = config["PayOSSettings:ReturnUrl"];
        _cancelUrl = config["PayOSSettings:CancelUrl"];
        _logger.LogInformation("PayOSService initialized with ReturnUrl: {ReturnUrl}, CancelUrl: {CancelUrl}", _returnUrl, _cancelUrl);
    }

    public async Task<PaymentResponse> CreatePaymentLink(PaymentRequest request, int accountId)
    {
        try
        {
            _logger.LogInformation($"Creating payment link for order: {request.OrderCode}, amount: {request.Amount}, accountId: {accountId}");

            // Tạo orderCode tăng dần, sử dụng InitialOrderCode
            var lastOrderCode = await _paymentTransactionRepository.GetLastOrderCodeAsync();
            long newOrderCode = lastOrderCode == 0 
                ? InitialOrderCode 
                : Math.Max(lastOrderCode + 1, InitialOrderCode);
            request.OrderCode = newOrderCode;

            var items = request.Items ?? new List<ItemData>();
            var paymentData = new PaymentData(
                orderCode: request.OrderCode,
                amount: request.Amount,
                description: request.Description,
                items: items,
                returnUrl: _returnUrl,
                cancelUrl: _cancelUrl,
                buyerName: request.BuyerName,
                buyerEmail: request.BuyerEmail,
                buyerPhone: request.BuyerPhone,
                expiredAt: request.ExpiredAt ?? (int)(DateTime.UtcNow.AddDays(7) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
            );

            _logger.LogDebug($"Sending payment data to PayOS: {JsonSerializer.Serialize(paymentData)}");

            CreatePaymentResult createPaymentResult = await _payOS.createPaymentLink(paymentData);
            _logger.LogDebug($"PayOS response: {JsonSerializer.Serialize(createPaymentResult)}");

            if (createPaymentResult.status != "PENDING")
            {
                _logger.LogError($"PayOS returned error - Status: {createPaymentResult.status}, Description: {createPaymentResult.description}");
                throw new Exception($"PayOS error - Status: {createPaymentResult.status}, Description: {createPaymentResult.description}");
            }

            var transaction = new PaymentTransaction
            {
                OrderCode = request.OrderCode,
                Amount = request.Amount,
                Description = request.Description,
                Status = "PENDING",
                AccountId = accountId,
                PaymentLinkId = createPaymentResult.paymentLinkId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentTransactionRepository.AddAsync(transaction);

            _logger.LogInformation($"Payment link created: {createPaymentResult.checkoutUrl}");

            return new PaymentResponse
            {
                Code = createPaymentResult.status,
                Desc = createPaymentResult.description,
                Data = new PaymentResponseData
                {
                    PaymentLinkId = createPaymentResult.paymentLinkId,
                    CheckoutUrl = createPaymentResult.checkoutUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to create payment link for order: {request.OrderCode}");
            throw new Exception($"Failed to create payment link: {ex.Message}", ex);
        }
    }

    public async Task<PaymentLinkInformation> GetPaymentLinkInformation(long orderCode)
    {
        try
        {
            _logger.LogInformation($"Fetching payment link information for order: {orderCode}");
            var result = await _payOS.getPaymentLinkInformation(orderCode);
            _logger.LogInformation($"Payment link information fetched: {result.id}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to fetch payment link information for order: {orderCode}");
            throw new Exception($"Failed to fetch payment link information: {ex.Message}");
        }
    }

    public async Task<PaymentLinkInformation> CancelPaymentLink(long orderCode, string cancellationReason = null)
    {
        try
        {
            _logger.LogInformation($"Canceling payment link for order: {orderCode}");
            var result = await _payOS.cancelPaymentLink(orderCode, cancellationReason);
            _logger.LogInformation($"Payment link canceled: {result.id}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to cancel payment link for order: {orderCode}");
            throw new Exception($"Failed to cancel payment link: {ex.Message}");
        }
    }

    public WebhookData VerifyWebhookData(WebhookType webhookBody)
    {
        try
        {
            _logger.LogInformation("Verifying webhook data");
            var webhookData = _payOS.verifyPaymentWebhookData(webhookBody);
            _logger.LogInformation($"Webhook data verified for order: {webhookData.orderCode}");
            return webhookData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify webhook data");
            throw new Exception($"Failed to verify webhook data: {ex.Message}");
        }
    }

    public async Task<List<PaymentHistory>> GetPaymentTransactionByAccountId(int accountId)
    {
        var result = await _paymentTransactionRepository.GetByAccountIdAsync(accountId);
        var paymentHistory = new List<PaymentHistory>();

        foreach (var ph in result)
        {
            paymentHistory.Add(new PaymentHistory 
            { 
                Description = ph.Description, 
                Amount = ph.Amount, 
                Status = ph.Status, 
                CreatedAt = ph.CreatedAt 
            });
        }
        
        return paymentHistory;
    }

    public async Task<AccountDto?> GetAccountByIdAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null) return null;
        return new AccountDto 
        { 
            Id = account.Id, 
            FullName = account.FullName, 
            PhoneNumber = account.PhoneNumber 
        };
    }

    public async Task<PaymentTransactionDto?> GetTransactionByOrderCodeAsync(long orderCode)
    {
        var transaction = await _paymentTransactionRepository.GetByOrderCodeAsync(orderCode);
        if (transaction == null) return null;
        return new PaymentTransactionDto 
        { 
            Id = transaction.Id,
            OrderCode = transaction.OrderCode,
            Amount = transaction.Amount,
            Status = transaction.Status,
            CreatedAt = transaction.CreatedAt,
            AccountId = transaction.AccountId
        };
    }

    public async Task UpdateTransactionStatusAsync(long orderCode, string status)
    {
        var transaction = await _paymentTransactionRepository.GetByOrderCodeAsync(orderCode);
        if (transaction == null)
        {
            throw new Exception($"Transaction with orderCode {orderCode} not found.");
        }

        transaction.Status = status;
        transaction.UpdatedAt = DateTime.UtcNow;
        await _paymentTransactionRepository.UpdateAsync(transaction);
    }

    public async Task UpdateAccountSubscriptionAsync(int accountId, int amount)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
        {
            throw new Exception($"Account with ID {accountId} not found.");
        }

        // Cập nhật subscription dựa trên amount (thêm trường vào Account nếu cần)
        // account.SubscriptionId = amount switch
        // {
        //     10000 => 2, // vip1
        //     15000 => 3, // vip2
        //     _ => account.SubscriptionId // Giữ nguyên nếu không khớp
        // };
        // 
        // account.StartDate = DateTime.UtcNow;
        // account.EndDate = DateTime.UtcNow.AddDays(30);
        // 
        // await _accountRepository.UpdateAsync(account);

        _logger.LogInformation($"Subscription updated for Account ID {accountId} based on amount {amount}.");
    }
}

public class PayOSCredentials
{
    public string ClientId { get; set; }
    public string ApiKey { get; set; }
    public string ChecksumKey { get; set; }
}