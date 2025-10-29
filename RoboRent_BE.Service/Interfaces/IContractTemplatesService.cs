using RoboRent_BE.Model.DTOS.ContractTemplates;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IContractTemplatesService
{
    Task<IEnumerable<ContractTemplatesResponse>> GetAllContractTemplatesAsync();
    Task<ContractTemplatesResponse?> GetContractTemplatesByIdAsync(int id);
    Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByStatusAsync(string status);
    Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByCreatedByAsync(int createdBy);
    Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByVersionAsync(string version);
    Task<ContractTemplatesResponse> CreateContractTemplatesAsync(CreateContractTemplatesRequest request);
    Task<ContractTemplatesResponse?> UpdateContractTemplatesAsync(UpdateContractTemplatesRequest request);
    Task<bool> DeleteContractTemplatesAsync(int id);
}

