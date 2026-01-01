using Microsoft.AspNetCore.Http;
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
    Task<ContractDraftsResponse> CreateContractDraftsAsync(CreateContractDraftsRequest request, int staffId);
    Task<ContractDraftsResponse?> UpdateContractDraftsAsync(UpdateContractDraftsRequest request);
    Task<bool> DeleteContractDraftsAsync(int id);
    
    // New methods for contract workflow
    Task<ContractDraftsResponse?> ManagerSignContractAsync(int id, ManagerSignatureRequest request, int managerId);
    Task<ContractDraftsResponse?> CustomerSignContractAsync(int id, CustomerSignatureRequest request, int customerId);
    Task<ContractDraftsResponse?> ManagerCancelContractAsync(int id, ManagerCancelRequest request, int managerId);
    Task<ContractDraftsResponse?> CustomerRejectContractAsync(int id, CustomerRejectRequest request, int customerId);
    Task<ContractDraftsResponse?> CustomerRequestChangeAsync(int id, CustomerRequestChangeRequest request, int customerId);
    Task<IEnumerable<ContractDraftsResponse>> GetPendingManagerSignatureContractsAsync(int managerId);
    Task<IEnumerable<ContractDraftsResponse>> GetPendingCustomerSignatureContractsAsync(int customerId);
    Task<IEnumerable<ContractDraftsResponse>> GetChangeRequestedContractsAsync();
    Task<ContractDraftsResponse?> SendToManagerAsync(int id, int staffId);
    Task<ContractDraftsResponse?> ReviseContractAsync(int id, UpdateContractDraftsRequest request, int staffId);
    
    // OTP Verification methods for customer signing
    Task<bool> SendVerificationCodeAsync(int contractDraftId, int customerId);
    Task<bool> VerifyCodeAsync(int contractDraftId, string code, int customerId);
    
    // Method to update BodyJson when draft clause is updated
    Task UpdateBodyJsonFromDraftClauseAsync(int contractDraftId, int draftClauseId, string newTitle, string newBody);
    
    // Download contract as PDF or Word
    Task<byte[]> DownloadContractAsPdfAsync(int id, int userId);
    Task<byte[]> DownloadContractAsWordAsync(int id, int userId);
    
    // Customer sign contract with file upload and validation
    Task<ContractDraftsResponse?> CustomerSignContractWithFileAsync(int id, IFormFile signedContractFile, int customerId);
}

