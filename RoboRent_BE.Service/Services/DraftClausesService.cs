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

    public async Task<IEnumerable<DraftClausesResponse>> GetDraftClausesByIsModifiedAsync(int contractDraftId, bool isModified)
    {
        var draftClauses = await _draftClausesRepository.GetDraftClausesByIsModifiedAsync(contractDraftId, isModified);
        return _mapper.Map<IEnumerable<DraftClausesResponse>>(draftClauses);
    }

    public async Task<DraftClausesResponse> CreateDraftClausesAsync(CreateDraftClausesRequest request)
    {
        var draftClause = _mapper.Map<DraftClauses>(request);
        
        // When first creating a draft clause, it's an exact copy from template, so IsModified = false
        draftClause.IsModified = false;
        
        var createdDraftClause = await _draftClausesRepository.AddAsync(draftClause);
        
        return _mapper.Map<DraftClausesResponse>(createdDraftClause);
    }

    public async Task<DraftClausesResponse?> UpdateDraftClausesAsync(UpdateDraftClausesRequest request)
    {
        var existingDraftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == request.Id, "ContractDraft,TemplateClause");
        if (existingDraftClause == null)
            return null;

        // Check if the clause content is being changed (before mapping)
        bool contentChanged = existingDraftClause.Title != request.Title || 
                            existingDraftClause.Body != request.Body;

        // Get the related template clause to check IsMandatory and IsEditable
        if (existingDraftClause.TemplateClause != null)
        {
            var templateClause = existingDraftClause.TemplateClause;
            
            // If the clause is mandatory and NOT editable, prevent any changes to Title or Body
            if (templateClause.IsMandatory == true && templateClause.IsEditable == false && contentChanged)
            {
                throw new InvalidOperationException(
                    $"Cannot edit clause '{templateClause.Title}'. This is a mandatory clause that cannot be modified.");
            }
            
            // Even if not editable but also not mandatory, check for content changes
            if (templateClause.IsEditable == false && contentChanged)
            {
                throw new InvalidOperationException(
                    $"Cannot edit clause '{templateClause.Title}'. This clause is not editable.");
            }
        }

        // Map the request to existing entity (this updates Title, Body, etc.)
        _mapper.Map(request, existingDraftClause);
        
        // Set IsModified flag based on whether content actually changed
        // This happens AFTER mapping to ensure it's not overwritten
        if (contentChanged)
        {
            existingDraftClause.IsModified = true;
        }
        
        var updatedDraftClause = await _draftClausesRepository.UpdateAsync(existingDraftClause);
        
        return _mapper.Map<DraftClausesResponse>(updatedDraftClause);
    }

    public async Task<bool> DeleteDraftClausesAsync(int id)
    {
        var draftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == id, "TemplateClause");
        if (draftClause == null)
            return false;

        // Check if the clause is mandatory - mandatory clauses cannot be deleted
        if (draftClause.TemplateClause != null && draftClause.TemplateClause.IsMandatory == true)
        {
            throw new InvalidOperationException(
                $"Cannot delete clause '{draftClause.TemplateClause.Title}'. This is a mandatory clause that must be included in the draft.");
        }

        await _draftClausesRepository.DeleteAsync(draftClause);
        return true;
    }
}

