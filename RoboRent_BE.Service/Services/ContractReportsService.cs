using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoboRent_BE.Model.DTOS;
using RoboRent_BE.Model.DTOS.ContractReports;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interface;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ContractReportsService : IContractReportsService
{
    private readonly IContractReportsRepository _contractReportsRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IDraftClausesRepository _draftClausesRepository;
    private readonly IPayOSService _payOSService;
    private readonly UserManager<ModifyIdentityUser> _userManager;
    private readonly ILogger<ContractReportsService> _logger;

    public ContractReportsService(
        IContractReportsRepository contractReportsRepository,
        IAccountRepository accountRepository,
        IDraftClausesRepository draftClausesRepository,
        IPayOSService payOSService,
        UserManager<ModifyIdentityUser> userManager,
        ILogger<ContractReportsService> logger)
    {
        _contractReportsRepository = contractReportsRepository;
        _accountRepository = accountRepository;
        _draftClausesRepository = draftClausesRepository;
        _payOSService = payOSService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<ContractReportResponse>> GetAllContractReportsAsync()
    {
        var reports = await _contractReportsRepository.GetAllAsync(
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");
        
        return reports.Select(MapToResponse).ToList();
    }

    public async Task<ContractReportResponse?> GetContractReportByIdAsync(int id)
    {
        var report = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");
        
        if (report == null)
            return null;

        return MapToResponse(report);
    }

    public async Task<IEnumerable<ContractReportResponse>> GetPendingContractReportsAsync()
    {
        var reports = await _contractReportsRepository.GetPendingReportsAsync();
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
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");

        return MapToResponse(reportWithIncludes);
    }

    public async Task<ContractReportResponse> ResolveContractReportAsync(
        int id, 
        ResolveContractReportRequest request, 
        int managerId)
    {
        // var report = await _contractReportsRepository.GetAsync(
        //     cr => cr.Id == id,
        //     includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");
        //
        // if (report == null)
        //     throw new Exception("Contract report not found");
        //
        // if (report.Status != "Pending")
        //     throw new Exception($"Cannot resolve report with status: {report.Status}");
        //
        // // Get accused account for payment
        // var accused = await _accountRepository.GetByIdAsync(report.AccusedId ?? 0);
        // if (accused == null)
        //     throw new Exception("Accused account not found");
        //
        // // Get user email from ModifyIdentityUser
        // var user = await _userManager.FindByIdAsync(accused.UserId ?? "");
        // var userEmail = user?.Email ?? "";
        //
        // // Create PayOS payment
        // var paymentRequest = new PaymentRequest
        // {
        //     OrderCode = 0, // Will be auto-generated by PayOSService
        //     Amount = 100000, // Default amount - adjust as needed
        //     Description = $"Payment for Contract Report #{id} Resolution",
        //     BuyerName = accused.FullName ?? "Unknown",
        //     BuyerEmail = userEmail,
        //     BuyerPhone = accused.PhoneNumber ?? "",
        //     Items = new List<Net.payOS.Types.ItemData>
        //     {
        //         new Net.payOS.Types.ItemData(
        //             name: $"Contract Report Resolution #{id}",
        //             quantity: 1,
        //             price: 100000
        //         )
        //     }
        // };
        //
        // var paymentResponse = await _payOSService.CreatePaymentLink(paymentRequest, accused.Id);
        //
        // if (paymentResponse.Code != "PENDING")
        //     throw new Exception($"Failed to create payment link: {paymentResponse.Desc}");
        //
        // // Get the payment transaction to get its ID
        // var dbContext = _contractReportsRepository.GetDbContext();
        // var paymentTransaction = await dbContext.PaymentTransactions
        //     .FirstOrDefaultAsync(pt => pt.PaymentLinkId == paymentResponse.Data.PaymentLinkId);
        //
        // if (paymentTransaction == null)
        //     throw new Exception("Payment transaction not found after creation");
        //
        // // Update report
        // report.Status = "Resolved";
        // report.Resolution = request.Resolution;
        // report.ReviewedBy = managerId;
        // report.ReviewedAt = DateTime.UtcNow;
        // report.PaymentId = paymentTransaction.Id;
        //
        // await _contractReportsRepository.UpdateAsync(report);
        //
        // // Reload with includes
        // var updatedReport = await _contractReportsRepository.GetAsync(
        //     cr => cr.Id == id,
        //     includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");
        //
        // var response = MapToResponse(updatedReport);
        // response.PaymentLink = paymentResponse.Data.CheckoutUrl;
        //
        // return response;
        return new ContractReportResponse();
    }

    public async Task<ContractReportResponse> RejectContractReportAsync(
        int id, 
        RejectContractReportRequest request, 
        int managerId)
    {
        var report = await _contractReportsRepository.GetAsync(
            cr => cr.Id == id,
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");

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
            includeProperties: "DraftClauses,Account,Accused,Manager,PaymentTransaction");

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

