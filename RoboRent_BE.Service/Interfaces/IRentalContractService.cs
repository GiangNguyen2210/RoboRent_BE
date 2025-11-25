using RoboRent_BE.Model.DTOS.RentalContract;

namespace RoboRent_BE.Service.Interfaces;

public interface IRentalContractService
{
    Task<IEnumerable<RentalContractResponse>> GetAllRentalContractsAsync();
    Task<RentalContractResponse?> GetRentalContractByIdAsync(int id);
    Task<IEnumerable<RentalContractResponse>> GetRentalContractsByRentalIdAsync(int rentalId);
    Task<RentalContractResponse> CreateRentalContractAsync(CreateRentalContractRequest request);
    Task<RentalContractResponse?> UpdateRentalContractAsync(UpdateRentalContractRequest request);
    Task<bool> DeleteRentalContractAsync(int id);
}
