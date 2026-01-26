using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using RoboRent_BE.Model.DTOS.ContractReports;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ContractReportsService : IContractReportsService
{
    private readonly IContractReportsRepository _contractReportsRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IDraftClausesRepository _draftClausesRepository;
    private readonly IPaymentRecordRepository _paymentRecordRepository;
    private readonly PayOS _payOS;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;
    private readonly UserManager<ModifyIdentityUser> _userManager;
    private readonly ILogger<ContractReportsService> _logger;

    public ContractReportsService(
        IContractReportsRepository contractReportsRepository,
        IAccountRepository accountRepository,
        IDraftClausesRepository draftClausesRepository,
        IPaymentRecordRepository paymentRecordRepository,
        IConfiguration config,
        UserManager<ModifyIdentityUser> userManager,
        ILogger<ContractReportsService> logger)
    {
        _contractReportsRepository = contractReportsRepository;
        _accountRepository = accountRepository;
        _draftClausesRepository = draftClausesRepository;
        _paymentRecordRepository = paymentRecordRepository;
        _userManager = userManager;
        _logger = logger;

        // Initialize PayOS
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
    }

    public async Task<IEnumerable<ContractReportResponse>> GetAllContractReportsAsync()
    {
        var reports = await _contractReportsRepository.GetAllAsync(
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");
        
        return reports.Select(MapToResponse).ToList();
    }

    public async Task<ContractReportResponse?> GetContractReportByIdAsync(int id)
    {
        var report = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");
        
        if (report == null)
            return null;

        return MapToResponse(report);
    }

    public async Task<IEnumerable<ContractReportResponse>> GetPendingContractReportsAsync()
    {
        var reports = await _contractReportsRepository.GetPendingReportsAsync();
        return reports.Select(MapToResponse).ToList();
    }

    public async Task<IEnumerable<ContractReportResponse>> GetContractReportsByUserIdAsync(int userId)
    {
        var reports = await _contractReportsRepository.GetAllAsync(
            filter: cr => cr.ReporterId == userId,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");
        
        return reports.Select(MapToResponse).ToList();
    }

    public async Task<ContractReportResponse> CreateContractReportAsync(
        CreateContractReportRequest request, 
        int reporterId, 
        string reportRole)
    {
        // Validate accused exists
        var accused = await _accountRepository.GetByIdAsync(request.AccusedId);
        if (accused == null)
            throw new Exception("Accused account not found");

        // Validate draft clause exists if provided
        if (request.DraftClausesId.HasValue)
        {
            var draftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == request.DraftClausesId.Value);
            if (draftClause == null)
                throw new Exception("Draft clause not found");
        }

        var report = new ContractReports
        {
            DraftClausesId = request.DraftClausesId,
            ReporterId = reporterId,
            ReportRole = reportRole,
            AccusedId = request.AccusedId,
            Description = request.Description,
            EvidencePath = request.EvidencePath,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            ReviewedBy = null,
            ReviewedAt = null,
            PaymentId = null
        };

        var createdReport = await _contractReportsRepository.AddAsync(report);
        
        // Reload with includes
        var reportWithIncludes = await _contractReportsRepository.GetAsync(
            cr => cr.Id == createdReport.Id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");

        return MapToResponse(reportWithIncludes);
    }

    public async Task<ContractReportResponse> ResolveContractReportAsync(
        int id, 
        ResolveContractReportRequest request, 
        int managerId)
    {
        var report = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");

        if (report == null)
            throw new Exception("Contract report not found");

        if (report.Status != "Pending")
            throw new Exception($"Cannot resolve report with status: {report.Status}");

        // Get accused account for payment
        var accused = await _accountRepository.GetByIdAsync(report.AccusedId ?? 0);
        if (accused == null)
            throw new Exception("Accused account not found");

        // Get user email from ModifyIdentityUser
        var user = await _userManager.FindByIdAsync(accused.UserId ?? "");
        var userEmail = user?.Email ?? "";

        // Generate unique OrderCode
        var lastOrderCode = await _paymentRecordRepository.GetLastOrderCodeAsync();
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long newOrderCode = lastOrderCode == 0 ? timestamp : Math.Max(lastOrderCode + 1, timestamp);

        // Create PayOS payment link
        var paymentAmount = 1000; 
        var paymentData = new PaymentData(
            orderCode: newOrderCode,
            amount: paymentAmount,
            description: $"Report #{id} Resolution",
            items: new List<ItemData>
            {
                new ItemData("Report Resolution", 1, paymentAmount)
            },
            returnUrl: _returnUrl,
            cancelUrl: _cancelUrl,
            buyerName: accused.FullName ?? "Unknown",
            buyerPhone: accused.PhoneNumber ?? "",
            expiredAt: (int)(DateTime.UtcNow.AddDays(7) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        );

        _logger.LogInformation($"Creating PayOS payment link for Contract Report Resolution - OrderCode: {newOrderCode}");
        var createPaymentResult = await _payOS.createPaymentLink(paymentData);

        if (createPaymentResult.status != "PENDING")
        {
            _logger.LogError($"PayOS returned error - Status: {createPaymentResult.status}");
            throw new Exception($"PayOS error: {createPaymentResult.description}");
        }

        // Calculate ExpiredAt from the expiredAt timestamp in paymentData
        var expiredAt = DateTimeOffset.FromUnixTimeSeconds((long)paymentData.expiredAt).UtcDateTime;

       
        // If customer reported → "Refund", else → "Fine"
        var paymentType = report.ReportRole == "Customer" ? "Refund" : "Fine";

        // Save PaymentRecord
        var paymentRecord = new PaymentRecord
        {
            RentalId =  null, // Contract reports don't have rental
            PaymentType = paymentType,
            Amount = paymentAmount,
            OrderCode = newOrderCode,
            PaymentLinkId = createPaymentResult.paymentLinkId,
            Status = "Pending",
            CheckoutUrl = createPaymentResult.checkoutUrl,
            ExpiredAt = expiredAt,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRecordRepository.AddAsync(paymentRecord);

        // Update report
        report.Status = "Resolved";
        report.Resolution = request.Resolution;
        report.ReviewedBy = managerId;
        report.ReviewedAt = DateTime.UtcNow;
        report.PaymentId = paymentRecord.Id;

        await _contractReportsRepository.UpdateAsync(report);

        // Reload with includes
        var updatedReport = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");

        var response = MapToResponse(updatedReport);
        response.PaymentLink = createPaymentResult.checkoutUrl;

        return response;
    }

    public async Task<ContractReportResponse> RejectContractReportAsync(
        int id, 
        RejectContractReportRequest request, 
        int managerId)
    {
        var report = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");

        if (report == null)
            throw new Exception("Contract report not found");

        if (report.Status != "Pending")
            throw new Exception($"Cannot reject report with status: {report.Status}");

        // Update report
        report.Status = "Rejected";
        report.Resolution = request.Resolution;
        report.ReviewedBy = managerId;
        report.ReviewedAt = DateTime.UtcNow;

        await _contractReportsRepository.UpdateAsync(report);

        // Reload with includes
        var updatedReport = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentRecord");

        return MapToResponse(updatedReport);
    }

    public async Task<bool> DeleteContractReportAsync(int id)
    {
        var report = await _contractReportsRepository.GetAsync(cr => cr.Id == id);
        
        if (report == null)
            return false;

        await _contractReportsRepository.DeleteAsync(report);
        return true;
    }

    public async Task ExpirePendingReportsAsync()
    {
        var expiredReports = await _contractReportsRepository.GetReportsPendingExpirationAsync();
        
        foreach (var report in expiredReports)
        {
            report.Status = "Expired";
            await _contractReportsRepository.UpdateAsync(report);
            _logger.LogInformation($"Expired contract report #{report.Id} after 3 days");
        }
    }

    private ContractReportResponse MapToResponse(ContractReports report)
    {
        return new ContractReportResponse
        {
            Id = report.Id,
            DraftClausesId = report.DraftClausesId,
            DraftClauseTitle = report.DraftClauses?.Title,
            ReporterId = report.ReporterId,
            ReporterName = report.Account?.FullName,
            ReportRole = report.ReportRole,
            AccusedId = report.AccusedId,
            AccusedName = report.Accused?.FullName,
            Description = report.Description,
            EvidencePath = report.EvidencePath,
            Status = report.Status,
            Resolution = report.Resolution,
            CreatedAt = report.CreatedAt,
            ReviewedBy = report.ReviewedBy,
            ReviewerName = report.Manager?.FullName,
            ReviewedAt = report.ReviewedAt,
            PaymentId = report.PaymentId,
            PaymentLink = null // Will be set separately for resolved reports
        };
    }
}

