using RoboRent_BE.Model.DTOS.ContractReports;

namespace RoboRent_BE.Service.Interfaces;

public interface IContractReportsService
{
    Task<IEnumerable<ContractReportResponse>> GetAllContractReportsAsync();
    Task<ContractReportResponse?> GetContractReportByIdAsync(int id);
    Task<IEnumerable<ContractReportResponse>> GetPendingContractReportsAsync();
    Task<ContractReportResponse> CreateContractReportAsync(CreateContractReportRequest request, int reporterId, string reportRole);
    Task<ContractReportResponse> ResolveContractReportAsync(int id, ResolveContractReportRequest request, int managerId);
    Task<ContractReportResponse> RejectContractReportAsync(int id, RejectContractReportRequest request, int managerId);
    Task<bool> DeleteContractReportAsync(int id);
    Task ExpirePendingReportsAsync();
}

