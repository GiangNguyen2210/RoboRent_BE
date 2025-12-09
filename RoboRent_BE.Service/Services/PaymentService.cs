using Microsoft.Extensions.Logging;
using RoboRent_BE.Model.DTOS;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interface;
using Net.payOS.Types;
using Net.payOS;
using Microsoft.Extensions.Configuration;

namespace RoboRent_BE.Service.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRecordRepository _paymentRecordRepo;
    private readonly IRentalRepository _rentalRepo;
    private readonly IPriceQuoteRepository _priceQuoteRepo;
    private readonly IAccountRepository _accountRepo;
    private readonly IGroupScheduleRepository _groupScheduleRepo;
    private readonly IActualDeliveryRepository _actualDeliveryRepo;
    private readonly PayOS _payOS;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRecordRepository paymentRecordRepo,
        IRentalRepository rentalRepo,
        IPriceQuoteRepository priceQuoteRepo,
        IAccountRepository accountRepo,
        IGroupScheduleRepository groupScheduleRepo,
        IActualDeliveryRepository actualDeliveryRepo,
        IConfiguration config,
        ILogger<PaymentService> logger)
    {
        _paymentRecordRepo = paymentRecordRepo;
        _rentalRepo = rentalRepo;
        _priceQuoteRepo = priceQuoteRepo;
        _accountRepo = accountRepo;
        _groupScheduleRepo = groupScheduleRepo;
        _actualDeliveryRepo = actualDeliveryRepo;
        _logger = logger;

        // ✅ Inject PayOS trực tiếp
        var clientId = config["PayOSCredentials:ClientId"];
        var apiKey = config["PayOSCredentials:ApiKey"];
        var checksumKey = config["PayOSCredentials:ChecksumKey"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
        {
            throw new ArgumentNullException("PayOSCredentials", "PayOS credentials are not configured");
        }

        _payOS = new PayOS(clientId, apiKey, checksumKey);
        _returnUrl = config["PayOSSettings:ReturnUrl"];
        _cancelUrl = config["PayOSSettings:CancelUrl"];
        
        _logger.LogInformation("✅ PaymentService initialized with PayOS SDK");
    }

    #region Public Methods

    public async Task<PaymentRecordResponse> CreateDepositPaymentAsync(int rentalId)
    {
        var rental = await ValidateRentalForDepositAsync(rentalId);

        var existingDeposit = await _paymentRecordRepo.GetByRentalIdAndTypeAsync(rentalId, "Deposit");
        if (existingDeposit != null)
        {
            if (existingDeposit.Status == "Pending")
            {
                _logger.LogInformation($"Returning existing pending deposit for Rental {rentalId}");
                return MapToResponse(existingDeposit, rental.EventName);
            }
            if (existingDeposit.Status == "Paid")
            {
                throw new Exception($"Deposit payment already paid for Rental {rentalId}");
            }
        }

        var priceQuote = await GetApprovedPriceQuoteAsync(rentalId);
        int depositAmount = CalculateDepositAmount(priceQuote);
        var customer = rental.Account ?? throw new Exception($"Customer not found for Rental {rentalId}");

        var (orderCode, checkoutUrl, paymentLinkId, expiredAt) = await CreatePayOSPaymentLinkAsync(
            amount: depositAmount,
            description: $"Deposit R#{rentalId}",
            itemName: "Deposit Payment",
            customerName: customer.FullName,
            customerPhone: customer.PhoneNumber ?? "",
            daysValid: 7
        );

        var paymentRecord = new PaymentRecord
        {
            RentalId = rentalId,
            PriceQuoteId = priceQuote.Id,
            PaymentType = "Deposit",
            Amount = depositAmount,
            OrderCode = orderCode,
            PaymentLinkId = paymentLinkId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            CheckoutUrl = checkoutUrl,
            ExpiredAt = expiredAt
        };

        await _paymentRecordRepo.AddAsync(paymentRecord);
        _logger.LogInformation($"✅ Deposit payment created: OrderCode {orderCode}, Amount {depositAmount:N0} VND");

        return MapToResponse(paymentRecord, rental.EventName);
    }

    public async Task<PaymentRecordResponse> CreateFullPaymentAsync(int rentalId)
    {
        var rental = await ValidateRentalForFullPaymentAsync(rentalId);

        var existingFull = await _paymentRecordRepo.GetByRentalIdAndTypeAsync(rentalId, "Full");
        if (existingFull != null)
        {
            if (existingFull.Status == "Pending")
            {
                _logger.LogInformation($"Returning existing pending full payment for Rental {rentalId}");
                return MapToResponse(existingFull, rental.EventName);
            }
            if (existingFull.Status == "Paid")
            {
                throw new Exception($"Full payment already paid for Rental {rentalId}");
            }
        }

        await VerifyDepositPaidAsync(rentalId);
        var priceQuote = await GetApprovedPriceQuoteAsync(rentalId);
        int fullAmount = CalculateFullAmount(priceQuote);
        var customer = rental.Account ?? throw new Exception($"Customer not found for Rental {rentalId}");

        var (orderCode, checkoutUrl, paymentLinkId, expiredAt) = await CreatePayOSPaymentLinkAsync(
            amount: fullAmount,
            description: $"Full pay R#{rentalId}",
            itemName: "Full Payment",
            customerName: customer.FullName,
            customerPhone: customer.PhoneNumber ?? "",
            daysValid: 7
        );

        var paymentRecord = new PaymentRecord
        {
            RentalId = rentalId,
            PriceQuoteId = priceQuote.Id,
            PaymentType = "Full",
            Amount = fullAmount,
            OrderCode = orderCode,
            PaymentLinkId = paymentLinkId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            CheckoutUrl = checkoutUrl,
            ExpiredAt = expiredAt
        };

        await _paymentRecordRepo.AddAsync(paymentRecord);
        _logger.LogInformation($"✅ Full payment created: OrderCode {orderCode}, Amount {fullAmount:N0} VND");

        return MapToResponse(paymentRecord, rental.EventName);
    }

    public async Task<List<PaymentRecordResponse>> GetPaymentsByRentalIdAsync(int rentalId)
    {
        var payments = await _paymentRecordRepo.GetByRentalIdAsync(rentalId);
        var rental = await _rentalRepo.GetAsync(r => r.Id == rentalId);
        var rentalName = rental?.EventName;

        return payments.Select(p => MapToResponse(p, rentalName)).ToList();
    }

    public async Task ProcessWebhookAsync(long orderCode, string paymentStatus)
    {
        var paymentRecord = await _paymentRecordRepo.GetByOrderCodeAsync(orderCode);
        
        if (paymentRecord == null)
        {
            _logger.LogWarning($"⚠️ PaymentRecord not found for OrderCode {orderCode}");
            throw new Exception($"PaymentRecord not found for OrderCode {orderCode}");
        }

        if (paymentRecord.Status == paymentStatus)
        {
            _logger.LogInformation($"⏭️ PaymentRecord {orderCode} already processed with status {paymentStatus}");
            return;
        }

        paymentRecord.Status = paymentStatus;
        paymentRecord.UpdatedAt = DateTime.UtcNow;
        
        if (paymentStatus == "Paid")
        {
            paymentRecord.PaidAt = DateTime.UtcNow;
        }

        await _paymentRecordRepo.UpdateAsync(paymentRecord);
        _logger.LogInformation($"✅ Updated PaymentRecord {orderCode} to {paymentStatus}");

        if (paymentStatus == "Paid" && paymentRecord.PaymentType == "Deposit" && paymentRecord.RentalId.HasValue)
        {
            await HandleDepositPaidAsync(paymentRecord.RentalId.Value);
        }
    }

    public async Task<List<PaymentRecordResponse>> GetCustomerTransactionsAsync(int customerId)
    {
        var allPayments = await _paymentRecordRepo.GetAllAsync(
            filter: p => p.Rental != null && p.Rental.AccountId == customerId,
            includeProperties: "Rental"
        );

        return allPayments
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToResponse(p, p.Rental?.EventName))
            .ToList();
    }

    // ✅ NEW: Verify webhook signature
    public WebhookData VerifyPaymentWebhook(WebhookType webhookBody)
    {
        try
        {
            _logger.LogDebug("🔐 Verifying webhook signature");
            var webhookData = _payOS.verifyPaymentWebhookData(webhookBody);
            _logger.LogInformation($"✅ Webhook verified - OrderCode: {webhookData.orderCode}");
            return webhookData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Webhook signature verification failed");
            throw new Exception($"Invalid webhook signature: {ex.Message}", ex);
        }
    }

    // ✅ NEW: Get payment info from PayOS
    public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode)
    {
        try
        {
            _logger.LogInformation($"🔍 Fetching payment info for OrderCode: {orderCode}");
            var result = await _payOS.getPaymentLinkInformation(orderCode);
            _logger.LogInformation($"✅ Payment info fetched - Status: {result.status}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Failed to fetch payment info for OrderCode: {orderCode}");
            throw new Exception($"Failed to fetch payment link information: {ex.Message}", ex);
        }
    }

    // ✅ NEW: Cancel payment link
    public async Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderCode, string cancellationReason = null)
    {
        try
        {
            _logger.LogInformation($"🚫 Cancelling payment for OrderCode: {orderCode}");
            var result = await _payOS.cancelPaymentLink(orderCode, cancellationReason);
            _logger.LogInformation($"✅ Payment cancelled");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Failed to cancel payment for OrderCode: {orderCode}");
            throw new Exception($"Failed to cancel payment link: {ex.Message}", ex);
        }
    }
    
    public async Task ExpirePendingPaymentsAsync()
    {
        var expiredPayments = await _paymentRecordRepo.GetExpiredPaymentRecordsAsync();
        
        foreach (var payment in expiredPayments)
        {
            payment.Status = "Expired";
            payment.UpdatedAt = DateTime.UtcNow;
            await _paymentRecordRepo.UpdateAsync(payment);
            _logger.LogInformation($"Expired payment record #{payment.Id} (OrderCode: {payment.OrderCode})");
        }
        
        if (expiredPayments.Any())
        {
            _logger.LogInformation($"✅ Expired {expiredPayments.Count()} payment record(s)");
        }
    }

    #endregion

    #region Private Helper Methods - Validation

    private async Task<Rental> ValidateRentalForDepositAsync(int rentalId)
    {
        var rental = await _rentalRepo.GetAsync(
            r => r.Id == rentalId,
            includeProperties: "Account");

        if (rental == null)
            throw new Exception($"Rental {rentalId} not found");

        if (rental.Status != "PendingDeposit")
            throw new Exception($"Cannot create deposit payment. Rental status must be 'PendingDeposit'. Current: {rental.Status}");

        return rental;
    }

    private async Task<Rental> ValidateRentalForFullPaymentAsync(int rentalId)
    {
        var rental = await _rentalRepo.GetAsync(
            r => r.Id == rentalId,
            includeProperties: "Account");

        if (rental == null)
            throw new Exception($"Rental {rentalId} not found");

        if (rental.Status != "Completed")
            throw new Exception($"Cannot create full payment. Rental status must be 'Completed'. Current: {rental.Status}");

        return rental;
    }

    private async Task<PriceQuote> GetApprovedPriceQuoteAsync(int rentalId)
    {
        var priceQuote = await _priceQuoteRepo.GetAsync(pq => pq.RentalId == rentalId && pq.Status == "Approved");
        
        if (priceQuote == null)
            throw new Exception($"No approved PriceQuote found for Rental {rentalId}");

        return priceQuote;
    }

    private async Task VerifyDepositPaidAsync(int rentalId)
    {
        var depositPayment = await _paymentRecordRepo.GetByRentalIdAndTypeAsync(rentalId, "Deposit");
        
        if (depositPayment == null || depositPayment.Status != "Paid")
            throw new Exception($"Deposit payment must be paid before creating full payment");
    }

    #endregion

    #region Private Helper Methods - Calculation

    private int CalculateDepositAmount(PriceQuote priceQuote)
    {
        decimal total = (decimal)((priceQuote.Delivery ?? 0) + (priceQuote.Deposit ?? 0) + 
                                  (priceQuote.Complete ?? 0) + (priceQuote.Service ?? 0));

        if (total <= 0)
            throw new Exception("Total amount must be greater than 0");

        int depositAmount = (int)Math.Round(total * 0.3m);

        if (depositAmount < 1000)
            throw new Exception($"Deposit amount too small: {depositAmount} VND (minimum: 1,000 VND)");

        return depositAmount;
    }

    private int CalculateFullAmount(PriceQuote priceQuote)
    {
        decimal total = (decimal)((priceQuote.Delivery ?? 0) + (priceQuote.Deposit ?? 0) + 
                                  (priceQuote.Complete ?? 0) + (priceQuote.Service ?? 0));

        if (total <= 0)
            throw new Exception("Total amount must be greater than 0");

        int fullAmount = (int)Math.Round(total * 0.7m);

        if (fullAmount < 1000)
            throw new Exception($"Full amount too small: {fullAmount} VND (minimum: 1,000 VND)");

        return fullAmount;
    }

    #endregion

    #region Private Helper Methods - PayOS Integration

    private long GenerateOrderCode()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int random = new Random().Next(1000, 9999);
        return timestamp * 10000 + random;
    }

    private static int GetUnixTimestamp(DateTime dateTime)
    {
        return (int)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    private async Task<(long, string, string, DateTime)> CreatePayOSPaymentLinkAsync(
        int amount,
        string description,
        string itemName,
        string customerName,
        string customerPhone,
        int daysValid = 7)
    {
        long orderCode = GenerateOrderCode();
        var expiredAt = DateTime.UtcNow.AddDays(daysValid);
        int expiredAtUnix = GetUnixTimestamp(expiredAt);

        var paymentData = new PaymentData(
            orderCode: orderCode,
            amount: amount,
            description: description,
            items: new List<ItemData> 
            { 
                new ItemData(itemName, 1, amount) 
            },
            returnUrl: _returnUrl,
            cancelUrl: _cancelUrl,
            buyerName: customerName,
            buyerPhone: customerPhone,
            expiredAt: expiredAtUnix
        );

        _logger.LogInformation($"🔄 Creating PayOS payment link - OrderCode: {orderCode}, Amount: {amount:N0} VND");

        try
        {
            var result = await _payOS.createPaymentLink(paymentData);

            if (result.status != "PENDING")
            {
                _logger.LogError($"❌ PayOS error - Status: {result.status}, Description: {result.description}");
                throw new Exception($"PayOS error: {result.description}");
            }

            _logger.LogInformation($"✅ PayOS payment link created - PaymentLinkId: {result.paymentLinkId}");

            return (orderCode, result.checkoutUrl, result.paymentLinkId, expiredAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Failed to create PayOS payment link for OrderCode {orderCode}");
            throw new Exception($"Failed to create payment link: {ex.Message}", ex);
        }
    }

    #endregion

    #region Private Helper Methods - Business Logic

    private async Task HandleDepositPaidAsync(int rentalId)
    {
        var rental = await _rentalRepo.GetAsync(r => r.Id == rentalId);
        if (rental == null)
        {
            _logger.LogError($"❌ Rental {rentalId} not found when processing deposit payment");
            return;
        }

        rental.Status = "DeliveryScheduled";
        await _rentalRepo.UpdateAsync(rental);
        _logger.LogInformation($"✅ Rental {rentalId} status updated to DeliveryScheduled");

        var groupSchedule = await _groupScheduleRepo.GetAsync(gs => gs.RentalId == rentalId);
        if (groupSchedule == null)
        {
            _logger.LogError($"❌ GroupSchedule not found for Rental {rentalId}");
            return;
        }

        var existingDelivery = await _actualDeliveryRepo.GetByGroupScheduleIdAsync(groupSchedule.Id);
        if (existingDelivery != null)
        {
            _logger.LogInformation($"⏭️ ActualDelivery already exists for GroupSchedule {groupSchedule.Id}");
            return;
        }

        var actualDelivery = new ActualDelivery
        {
            GroupScheduleId = groupSchedule.Id,
            StaffId = null,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _actualDeliveryRepo.AddAsync(actualDelivery);
        
        groupSchedule.Status = "scheduled";
        await _groupScheduleRepo.UpdateAsync(groupSchedule);

        _logger.LogInformation($"✅ ActualDelivery created for GroupSchedule {groupSchedule.Id} after deposit payment");
    }

    #endregion

    #region Private Helper Methods - Mapping

    private PaymentRecordResponse MapToResponse(PaymentRecord p, string? rentalName = null)
    {
        return new PaymentRecordResponse
        {
            Id = p.Id,
            RentalId = p.RentalId,
            RentalName = rentalName,
            PriceQuoteId = p.PriceQuoteId,
            PaymentType = p.PaymentType,
            Amount = p.Amount,
            OrderCode = p.OrderCode,
            PaymentLinkId = p.PaymentLinkId,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
            PaidAt = p.PaidAt,
            CheckoutUrl = p.CheckoutUrl,
            ExpiredAt = p.ExpiredAt
        };
    }

    #endregion
}