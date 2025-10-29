using AutoMapper;
using RoboRent_BE.Model.DTOS.TemplateClauses;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class TemplateClausesService : ITemplateClausesService
{
    private readonly ITemplateClausesRepository _templateClausesRepository;
    private readonly IMapper _mapper;

    public TemplateClausesService(ITemplateClausesRepository templateClausesRepository, IMapper mapper)
    {
        _templateClausesRepository = templateClausesRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetAllTemplateClausesAsync()
    {
        var templateClauses = await _templateClausesRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<TemplateClausesResponse?> GetTemplateClausesByIdAsync(int id)
    {
        var templateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == id, "ContractTemplate");
        if (templateClause == null)
            return null;

        return _mapper.Map<TemplateClausesResponse>(templateClause);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByContractTemplateIdAsync(int contractTemplateId)
    {
        var templateClauses = await _templateClausesRepository.GetTemplateClausesByContractTemplateIdAsync(contractTemplateId);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByIsMandatoryAsync(bool isMandatory)
    {
        var templateClauses = await _templateClausesRepository.GetTemplateClausesByIsMandatoryAsync(isMandatory);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByIsEditableAsync(bool isEditable)
    {
        var templateClauses = await _templateClausesRepository.GetTemplateClausesByIsEditableAsync(isEditable);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<TemplateClausesResponse> CreateTemplateClausesAsync(CreateTemplateClausesRequest request)
    {
        var templateClause = _mapper.Map<TemplateClauses>(request);
        var createdTemplateClause = await _templateClausesRepository.AddAsync(templateClause);
        
        return _mapper.Map<TemplateClausesResponse>(createdTemplateClause);
    }

    public async Task<TemplateClausesResponse?> UpdateTemplateClausesAsync(UpdateTemplateClausesRequest request)
    {
        var existingTemplateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == request.Id, "ContractTemplate");
        if (existingTemplateClause == null)
            return null;

        _mapper.Map(request, existingTemplateClause);
        var updatedTemplateClause = await _templateClausesRepository.UpdateAsync(existingTemplateClause);
        
        return _mapper.Map<TemplateClausesResponse>(updatedTemplateClause);
    }

    public async Task<bool> DeleteTemplateClausesAsync(int id)
    {
        var templateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == id);
        if (templateClause == null)
            return false;

        await _templateClausesRepository.DeleteAsync(templateClause);
        return true;
    }
}

