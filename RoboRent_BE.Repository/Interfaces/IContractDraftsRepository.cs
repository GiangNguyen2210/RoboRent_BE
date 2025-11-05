using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IContractDraftsRepository : IGenericRepository<ContractDrafts>
{
    Task<IEnumerable<ContractDrafts>> GetContractDraftsByRentalIdAsync(int rentalId);
    Task<IEnumerable<ContractDrafts>> GetContractDraftsByStaffIdAsync(int staffId);
    Task<IEnumerable<ContractDrafts>> GetContractDraftsByManagerIdAsync(int managerId);
    Task<IEnumerable<ContractDrafts>> GetContractDraftsByStatusAsync(string status);
    Task<IEnumerable<ContractDrafts>> GetAllWithIncludesAsync();
}
