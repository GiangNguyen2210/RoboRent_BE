using AutoMapper;
using RoboRent_BE.Model.DTOS.DraftClauses;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class DraftClausesService : IDraftClausesService
{
    private readonly IDraftClausesRepository _draftClausesRepository;
    private readonly IMapper _mapper;

    public DraftClausesService(IDraftClausesRepository draftClausesRepository, IMapper mapper)
    {
        _draftClausesRepository = draftClausesRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DraftClausesResponse>> GetAllDraftClausesAsync()
    {
        var draftClauses = await _draftClausesRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<DraftClausesResponse>>(draftClauses);
    }

    public async Task<DraftClausesResponse?> GetDraftClausesByIdAsync(int id)
    {
        var draftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == id, "ContractDraft,TemplateClause");
        if (draftClause == null)
            return null;

        return _mapper.Map<DraftClausesResponse>(draftClause);
    }

    public async Task<IEnumerable<DraftClausesResponse>> GetDraftClausesByContractDraftIdAsync(int contractDraftId)
    {
        var draftClauses = await _draftClausesRepository.GetDraftClausesByContractDraftIdAsync(contractDraftId);
        return _mapper.Map<IEnumerable<DraftClausesResponse>>(draftClauses);
    }

    public async Task<IEnumerable<DraftClausesResponse>> GetDraftClausesByTemplateClauseIdAsync(int templateClauseId)
    {
        var draftClauses = await _draftClausesRepository.GetDraftClausesByTemplateClauseIdAsync(templateClauseId);
        return _mapper.Map<IEnumerable<DraftClausesResponse>>(draftClauses);
    }

    public async Task<DraftClausesResponse> CreateDraftClausesAsync(CreateDraftClausesRequest request)
    {
        var draftClause = _mapper.Map<DraftClauses>(request);
        var createdDraftClause = await _draftClausesRepository.AddAsync(draftClause);
        
        return _mapper.Map<DraftClausesResponse>(createdDraftClause);
    }

    public async Task<DraftClausesResponse?> UpdateDraftClausesAsync(UpdateDraftClausesRequest request)
    {
        var existingDraftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == request.Id, "ContractDraft,TemplateClause");
        if (existingDraftClause == null)
            return null;

        _mapper.Map(request, existingDraftClause);
        var updatedDraftClause = await _draftClausesRepository.UpdateAsync(existingDraftClause);
        
        return _mapper.Map<DraftClausesResponse>(updatedDraftClause);
    }

    public async Task<bool> DeleteDraftClausesAsync(int id)
    {
        var draftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == id);
        if (draftClause == null)
            return false;

        await _draftClausesRepository.DeleteAsync(draftClause);
        return true;
    }
}

