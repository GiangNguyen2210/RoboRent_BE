using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IContractReportsRepository : IGenericRepository<ContractReports>
{
    Task<IEnumerable<ContractReports>> GetPendingReportsAsync();
    Task<IEnumerable<ContractReports>> GetReportsPendingExpirationAsync();
}



