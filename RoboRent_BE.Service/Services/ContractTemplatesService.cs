using AutoMapper;
using RoboRent_BE.Model.DTOS.ContractTemplates;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ContractTemplatesService : IContractTemplatesService
{
    private readonly IContractTemplatesRepository _contractTemplatesRepository;
    private readonly IMapper _mapper;

    public ContractTemplatesService(IContractTemplatesRepository contractTemplatesRepository, IMapper mapper)
    {
        _contractTemplatesRepository = contractTemplatesRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetAllContractTemplatesAsync()
    {
        var contractTemplates = await _contractTemplatesRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<ContractTemplatesResponse?> GetContractTemplatesByIdAsync(int id)
    {
        var contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == id, "Created,Updated");
        if (contractTemplate == null)
            return null;

        return _mapper.Map<ContractTemplatesResponse>(contractTemplate);
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByStatusAsync(string status)
    {
        var contractTemplates = await _contractTemplatesRepository.GetContractTemplatesByStatusAsync(status);
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByCreatedByAsync(int createdBy)
    {
        var contractTemplates = await _contractTemplatesRepository.GetContractTemplatesByCreatedByAsync(createdBy);
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByVersionAsync(string version)
    {
        var contractTemplates = await _contractTemplatesRepository.GetContractTemplatesByVersionAsync(version);
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<ContractTemplatesResponse> CreateContractTemplatesAsync(CreateContractTemplatesRequest request)
    {
        var contractTemplate = _mapper.Map<ContractTemplates>(request);
        var createdContractTemplate = await _contractTemplatesRepository.AddAsync(contractTemplate);
        
        return _mapper.Map<ContractTemplatesResponse>(createdContractTemplate);
    }

    public async Task<ContractTemplatesResponse?> UpdateContractTemplatesAsync(UpdateContractTemplatesRequest request)
    {
        var existingContractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == request.Id, "Created,Updated");
        if (existingContractTemplate == null)
            return null;

        _mapper.Map(request, existingContractTemplate);
        existingContractTemplate.UpdatedAt = DateTime.UtcNow;
        var updatedContractTemplate = await _contractTemplatesRepository.UpdateAsync(existingContractTemplate);
        
        return _mapper.Map<ContractTemplatesResponse>(updatedContractTemplate);
    }

    public async Task<bool> DeleteContractTemplatesAsync(int id)
    {
        var contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == id);
        if (contractTemplate == null)
            return false;

        await _contractTemplatesRepository.DeleteAsync(contractTemplate);
        return true;
    }
}
