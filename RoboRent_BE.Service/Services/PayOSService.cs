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
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interface;

namespace RoboRent_BE.Service.Services;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;
    private readonly ILogger<PayOSService> _logger;
    private readonly IAccountRepository _accountRepository;
    private const long InitialOrderCode = 20; // Giá trị khởi đầu

    public PayOSService(
        IConfiguration config, 
        ILogger<PayOSService> logger, 
        IAccountRepository accountRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;

        var clientId = config["PayOSCredentials:ClientId"];
        var apiKey = config["PayOSCredentials:ApiKey"];
        var checksumKey = config["PayOSCredentials:ChecksumKey"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
        {
            throw new ArgumentNullException("PayOSCredentials", "PayOS credentials are not configured in appsettings.json");
        }

        _payOS = new PayOS(clientId, apiKey, checksumKey);

        _returnUrl = config["PayOSSettings:ReturnUrl"];
        _cancelUrl = config["PayOSSettings:CancelUrl"];
        _logger.LogInformation("PayOSService initialized with ReturnUrl: {ReturnUrl}, CancelUrl: {CancelUrl}", _returnUrl, _cancelUrl);
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
    
}