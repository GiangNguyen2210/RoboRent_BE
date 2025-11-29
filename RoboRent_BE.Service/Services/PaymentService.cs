using Microsoft.Extensions.Logging;
using RoboRent_BE.Model.DTOS;
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
    }

    public async Task<PaymentRecordResponse> CreateDepositPaymentAsync(int rentalId)
    {
        // 1. Validate Rental
        var rental = await _rentalRepo.GetAsync(
            r => r.Id == rentalId,
            includeProperties: "Account");

        if (rental == null)
            throw new Exception($"Rental {rentalId} not found");

        if (rental.Status != "ChuaThanhToan")
            throw new Exception($"Cannot create deposit payment. Rental status must be 'ChuaThanhToan'. Current: {rental.Status}");

        // 2. Check if deposit already exists
        var existingDeposit = await _paymentRecordRepo.GetByRentalIdAndTypeAsync(rentalId, "Deposit");
        if (existingDeposit != null)
            throw new Exception($"Deposit payment already exists for Rental {rentalId}");

        // 3. Get PriceQuote
        var priceQuote = await _priceQuoteRepo.GetAsync(pq => pq.RentalId == rentalId && pq.Status == "Approved");
        if (priceQuote == null)
            throw new Exception($"No approved PriceQuote found for Rental {rentalId}");

        // 4. Calculate Deposit Amount (30% of Total)
        var total = (priceQuote.Delivery ?? 0) + (priceQuote.Deposit ?? 0) + 
                    (priceQuote.Complete ?? 0) + (priceQuote.Service ?? 0);
        var depositAmount = (decimal)(total * 0.3);

        // 5. Get Customer info
        var customer = rental.Account;
        if (customer == null)
            throw new Exception($"Customer not found for Rental {rentalId}");

        // 6. Generate unique OrderCode
        var lastOrderCode = await _paymentRecordRepo.GetLastOrderCodeAsync();
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long newOrderCode = lastOrderCode == 0 ? timestamp : Math.Max(lastOrderCode + 1, timestamp);

        // 7. Create PayOS Payment Link
        var paymentData = new PaymentData(
            orderCode: newOrderCode,
            amount: (int)depositAmount,
            description: $"Deposit R#{rentalId}",  // ✅ NGẮN GỌN (< 25 chars)
            items: new List<ItemData> 
            { 
                new ItemData("Deposit", 1, (int)depositAmount) 
            },
            returnUrl: _returnUrl,
            cancelUrl: _cancelUrl,
            buyerName: customer.FullName,
            buyerPhone: customer.PhoneNumber ?? "",
            expiredAt: (int)(DateTime.UtcNow.AddDays(7) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        );

        _logger.LogInformation($"Creating PayOS payment link for Deposit - OrderCode: {newOrderCode}");
        var createPaymentResult = await _payOS.createPaymentLink(paymentData);

        if (createPaymentResult.status != "PENDING")
        {
            _logger.LogError($"PayOS returned error - Status: {createPaymentResult.status}");
            throw new Exception($"PayOS error: {createPaymentResult.description}");
        }

        // 8. Save PaymentRecord
        var paymentRecord = new PaymentRecord
        {
            RentalId = rentalId,
            PriceQuoteId = priceQuote.Id,
            PaymentType = "Deposit",
            Amount = depositAmount,
            OrderCode = newOrderCode,
            PaymentLinkId = createPaymentResult.paymentLinkId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRecordRepo.AddAsync(paymentRecord);

        _logger.LogInformation($"Deposit payment created: OrderCode {newOrderCode}, Amount {depositAmount}");

        return new PaymentRecordResponse
        {
            Id = paymentRecord.Id,
            RentalId = rentalId,
            PriceQuoteId = priceQuote.Id,
            PaymentType = "Deposit",
            Amount = depositAmount,
            OrderCode = newOrderCode,
            PaymentLinkId = createPaymentResult.paymentLinkId,
            Status = "Pending",
            CreatedAt = paymentRecord.CreatedAt,
            CheckoutUrl = createPaymentResult.checkoutUrl
        };
    }

    public async Task<PaymentRecordResponse> CreateFullPaymentAsync(int rentalId)
    {
        // 1. Validate Rental
        var rental = await _rentalRepo.GetAsync(
            r => r.Id == rentalId,
            includeProperties: "Account");

        if (rental == null)
            throw new Exception($"Rental {rentalId} not found");

        if (rental.Status != "Completed")
            throw new Exception($"Cannot create full payment. Rental status must be 'Completed'. Current: {rental.Status}");

        // 2. Check if full payment already exists
        var existingFull = await _paymentRecordRepo.GetByRentalIdAndTypeAsync(rentalId, "Full");
        if (existingFull != null)
            throw new Exception($"Full payment already exists for Rental {rentalId}");

        // 3. Verify Deposit is Paid
        var depositPayment = await _paymentRecordRepo.GetByRentalIdAndTypeAsync(rentalId, "Deposit");
        if (depositPayment == null || depositPayment.Status != "Paid")
            throw new Exception($"Deposit payment must be paid before creating full payment");

        // 4. Get PriceQuote
        var priceQuote = await _priceQuoteRepo.GetAsync(pq => pq.RentalId == rentalId && pq.Status == "Approved");
        if (priceQuote == null)
            throw new Exception($"No approved PriceQuote found for Rental {rentalId}");

        // 5. Calculate Full Amount (70% of Total)
        var total = (priceQuote.Delivery ?? 0) + (priceQuote.Deposit ?? 0) + 
                    (priceQuote.Complete ?? 0) + (priceQuote.Service ?? 0);
        var fullAmount = (decimal)(total * 0.7);

        // 6. Get Customer info
        var customer = rental.Account;
        if (customer == null)
            throw new Exception($"Customer not found for Rental {rentalId}");

        // 7. Generate unique OrderCode
        var lastOrderCode = await _paymentRecordRepo.GetLastOrderCodeAsync();
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long newOrderCode = lastOrderCode == 0 ? timestamp : Math.Max(lastOrderCode + 1, timestamp);

        // 8. Create PayOS Payment Link
        var paymentData = new PaymentData(
            orderCode: newOrderCode,
            amount: (int)fullAmount,
            description: $"Full pay R#{rentalId}",  // ✅ NGẮN GỌN (< 25 chars)
            items: new List<ItemData> 
            { 
                new ItemData("Full Payment", 1, (int)fullAmount) 
            },
            returnUrl: _returnUrl,
            cancelUrl: _cancelUrl,
            buyerName: customer.FullName,
            buyerPhone: customer.PhoneNumber ?? "",
            expiredAt: (int)(DateTime.UtcNow.AddDays(7) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        );

        _logger.LogInformation($"Creating PayOS payment link for Full - OrderCode: {newOrderCode}");
        var createPaymentResult = await _payOS.createPaymentLink(paymentData);

        if (createPaymentResult.status != "PENDING")
        {
            _logger.LogError($"PayOS returned error - Status: {createPaymentResult.status}");
            throw new Exception($"PayOS error: {createPaymentResult.description}");
        }

        // 9. Save PaymentRecord
        var paymentRecord = new PaymentRecord
        {
            RentalId = rentalId,
            PriceQuoteId = priceQuote.Id,
            PaymentType = "Full",
            Amount = fullAmount,
            OrderCode = newOrderCode,
            PaymentLinkId = createPaymentResult.paymentLinkId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRecordRepo.AddAsync(paymentRecord);

        _logger.LogInformation($"Full payment created: OrderCode {newOrderCode}, Amount {fullAmount}");

        return new PaymentRecordResponse
        {
            Id = paymentRecord.Id,
            RentalId = rentalId,
            PriceQuoteId = priceQuote.Id,
            PaymentType = "Full",
            Amount = fullAmount,
            OrderCode = newOrderCode,
            PaymentLinkId = createPaymentResult.paymentLinkId,
            Status = "Pending",
            CreatedAt = paymentRecord.CreatedAt,
            CheckoutUrl = createPaymentResult.checkoutUrl
        };
    }

    public async Task<List<PaymentRecordResponse>> GetPaymentsByRentalIdAsync(int rentalId)
    {
        var payments = await _paymentRecordRepo.GetByRentalIdAsync(rentalId);
        
        return payments.Select(p => new PaymentRecordResponse
        {
            Id = p.Id,
            RentalId = p.RentalId,
            PriceQuoteId = p.PriceQuoteId,
            PaymentType = p.PaymentType,
            Amount = p.Amount,
            OrderCode = p.OrderCode,
            PaymentLinkId = p.PaymentLinkId,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
            PaidAt = p.PaidAt
        }).ToList();
    }

    public async Task ProcessWebhookAsync(long orderCode, string paymentStatus)
    {
        var paymentRecord = await _paymentRecordRepo.GetByOrderCodeAsync(orderCode);
        
        if (paymentRecord == null)
        {
            _logger.LogWarning($"PaymentRecord not found for OrderCode {orderCode}");
            throw new Exception($"PaymentRecord not found for OrderCode {orderCode}");
        }

        // Check duplicate processing
        if (paymentRecord.Status == paymentStatus)
        {
            _logger.LogInformation($"PaymentRecord {orderCode} already processed with status {paymentStatus}");
            return;
        }

        // Update payment status
        paymentRecord.Status = paymentStatus;
        paymentRecord.UpdatedAt = DateTime.UtcNow;
        
        if (paymentStatus == "Paid")
        {
            paymentRecord.PaidAt = DateTime.UtcNow;
        }

        await _paymentRecordRepo.UpdateAsync(paymentRecord);
        _logger.LogInformation($"Updated PaymentRecord {orderCode} to {paymentStatus}");

        // Process business logic based on payment type
        if (paymentStatus == "Paid" && paymentRecord.PaymentType == "Deposit")
        {
            await HandleDepositPaidAsync(paymentRecord.RentalId);
        }
    }

    private async Task HandleDepositPaidAsync(int rentalId)
    {
        // 1. Update Rental status
        var rental = await _rentalRepo.GetAsync(r => r.Id == rentalId);
        if (rental == null)
        {
            _logger.LogError($"Rental {rentalId} not found when processing deposit payment");
            return;
        }

        rental.Status = "DeliveryScheduled";
        await _rentalRepo.UpdateAsync(rental);
        _logger.LogInformation($"Rental {rentalId} status updated to DeliveryScheduled");

        // 2. Get GroupSchedule
        var groupSchedule = await _groupScheduleRepo.GetAsync(gs => gs.RentalId == rentalId);
        if (groupSchedule == null)
        {
            _logger.LogError($"GroupSchedule not found for Rental {rentalId}");
            return;
        }

        // 3. Check if ActualDelivery already exists
        var existingDelivery = await _actualDeliveryRepo.GetByGroupScheduleIdAsync(groupSchedule.Id);
        if (existingDelivery != null)
        {
            _logger.LogInformation($"ActualDelivery already exists for GroupSchedule {groupSchedule.Id}");
            return;
        }

        // 4. Create ActualDelivery
        var actualDelivery = new ActualDelivery
        {
            GroupScheduleId = groupSchedule.Id,
            StaffId = null,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _actualDeliveryRepo.AddAsync(actualDelivery);
        
        // 5. Update GroupSchedule status
        groupSchedule.Status = "scheduled";
        await _groupScheduleRepo.UpdateAsync(groupSchedule);

        _logger.LogInformation($"ActualDelivery created for GroupSchedule {groupSchedule.Id} after deposit payment");
    }
}