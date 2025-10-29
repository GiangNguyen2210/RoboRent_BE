using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IContractTemplatesRepository : IGenericRepository<ContractTemplates>
{
    Task<IEnumerable<ContractTemplates>> GetContractTemplatesByStatusAsync(string status);
    Task<IEnumerable<ContractTemplates>> GetContractTemplatesByCreatedByAsync(int createdBy);
    Task<IEnumerable<ContractTemplates>> GetContractTemplatesByVersionAsync(string version);
    Task<IEnumerable<ContractTemplates>> GetAllWithIncludesAsync();
}

