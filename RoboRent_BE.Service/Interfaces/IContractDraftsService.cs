using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IContractDraftsService
{
    Task<IEnumerable<ContractDraftsResponse>> GetAllContractDraftsAsync();
    Task<ContractDraftsResponse?> GetContractDraftsByIdAsync(int id);
    Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByRentalIdAsync(int rentalId);
    Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByStaffIdAsync(int staffId);
    Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByManagerIdAsync(int managerId);
    Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByStatusAsync(string status);
    Task<ContractDraftsResponse> CreateContractDraftsAsync(CreateContractDraftsRequest request);
    Task<ContractDraftsResponse?> UpdateContractDraftsAsync(UpdateContractDraftsRequest request);
    Task<bool> DeleteContractDraftsAsync(int id);
}
