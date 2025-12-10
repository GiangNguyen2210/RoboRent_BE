using AutoMapper;
using RoboRent_BE.Model.DTOS.DraftClauses;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class DraftClausesService : IDraftClausesService
{
    private readonly IDraftClausesRepository _draftClausesRepository;
    private readonly ITemplateClausesRepository _templateClausesRepository;
    private readonly IContractDraftsRepository _contractDraftsRepository;
    private readonly IContractDraftsService _contractDraftsService;
    private readonly IMapper _mapper;

    public DraftClausesService(
        IDraftClausesRepository draftClausesRepository,
        ITemplateClausesRepository templateClausesRepository,
        IContractDraftsRepository contractDraftsRepository,
        IContractDraftsService contractDraftsService,
        IMapper mapper)
    {
        _draftClausesRepository = draftClausesRepository;
        _templateClausesRepository = templateClausesRepository;
        _contractDraftsRepository = contractDraftsRepository;
        _contractDraftsService = contractDraftsService;
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
        // If creating from a template, validate edit permissions
        if (request.TemplateClausesId.HasValue)
        {
            var templateClause = await _templateClausesRepository.GetByIdAsync(request.TemplateClausesId.Value);
            if (templateClause == null)
            {
                throw new InvalidOperationException("Template clause not found.");
            }

            // Check if the body or title is being changed from the template
            bool contentChanged = templateClause.Title != request.Title || 
                                 templateClause.Body != request.Body;

            if (contentChanged)
            {
                // If the clause is mandatory and NOT editable, prevent any changes
                if (templateClause.IsMandatory == true && templateClause.IsEditable == false)
                {
                    throw new InvalidOperationException(
                        $"Cannot modify clause '{templateClause.Title}' during creation. This is a mandatory clause that cannot be modified.");
                }
                
                // Even if not mandatory, if not editable, prevent changes
                if (templateClause.IsEditable == false)
                {
                    throw new InvalidOperationException(
                        $"Cannot modify clause '{templateClause.Title}' during creation. This clause is not editable.");
                }
            }
        }

        var draftClause = _mapper.Map<DraftClauses>(request);
        
        // When first creating a draft clause IsModified = false
        draftClause.IsModified = false;
        
        var createdDraftClause = await _draftClausesRepository.AddAsync(draftClause);
        
        return _mapper.Map<DraftClausesResponse>(createdDraftClause);
    }

    public async Task<CreateCustomDraftClauseResponse> CreateCustomDraftClauseAsync(CreateCustomDraftClauseRequest request)
    {
        int? createdTemplateClauseId = null;
        
        // Step 1: If saveAsTemplate = true, create template clause first
        if (request.SaveAsTemplate)
        {
            // Validate that ContractTemplatesId is provided
            if (!request.ContractTemplatesId.HasValue || request.ContractTemplatesId.Value == 0)
            {
                throw new InvalidOperationException(
                    "ContractTemplatesId is required when SaveAsTemplate is true.");
            }

            // Create the template clause
            var templateClause = new TemplateClauses
            {
                ClauseCode = request.ClauseCode,
                Title = request.Title,
                Body = request.Body,
                IsMandatory = request.IsMandatory ?? false,
                IsEditable = request.IsEditable ?? true,
                ContractTemplatesId = request.ContractTemplatesId.Value,
                CreatedAt = DateTime.UtcNow
            };

            var createdTemplateClause = await _templateClausesRepository.AddAsync(templateClause);
            createdTemplateClauseId = createdTemplateClause.Id;

            // Step 2: Create draft clause linked to the new template clause
            var draftClause = new DraftClauses
            {
                Title = request.Title,
                Body = request.Body,
                IsModified = false, // Fresh copy
                ContractDraftsId = request.ContractDraftsId,
                TemplateClausesId = createdTemplateClauseId,
                CreatedAt = DateTime.UtcNow
            };

            var createdDraftClause = await _draftClausesRepository.AddAsync(draftClause);

            return new CreateCustomDraftClauseResponse
            {
                DraftClause = _mapper.Map<DraftClausesResponse>(createdDraftClause),
                CreatedTemplateClauseId = createdTemplateClauseId,
                WasSavedAsTemplate = true,
                Message = "Custom clause created and saved as template successfully."
            };
        }
        else
        {
            // Step 2: Create draft clause only (no template clause)
            var draftClause = new DraftClauses
            {
                Title = request.Title,
                Body = request.Body,
                IsModified = false, // Fresh custom clause
                ContractDraftsId = request.ContractDraftsId,
                TemplateClausesId = null, // No template link
                CreatedAt = DateTime.UtcNow
            };

            var createdDraftClause = await _draftClausesRepository.AddAsync(draftClause);

            return new CreateCustomDraftClauseResponse
            {
                DraftClause = _mapper.Map<DraftClausesResponse>(createdDraftClause),
                CreatedTemplateClauseId = null,
                WasSavedAsTemplate = false,
                Message = "Custom clause created in draft only (not saved as template)."
            };
        }
    }

    public async Task<DraftClausesResponse?> UpdateDraftClausesAsync(UpdateDraftClausesRequest request)
    {
        var existingDraftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == request.Id, "ContractDraft,TemplateClause");
        if (existingDraftClause == null)
            return null;

        // Validate that the contract draft status allows updates
        var contractDraftId = existingDraftClause.ContractDraftsId ?? request.ContractDraftsId;
        if (contractDraftId.HasValue)
        {
            var contractDraft = await _contractDraftsRepository.GetAsync(cd => cd.Id == contractDraftId.Value);
            if (contractDraft == null)
            {
                throw new InvalidOperationException("Contract draft not found.");
            }

            // Prevent updates if contract draft status is "Active"
            var status = contractDraft.Status;
            if (status == "Active" || status == "Cancelled" || status == "Rejected")
            {
                throw new InvalidOperationException(
                    "Cannot update draft clause.");
            }

            // Only allow updates when contract draft status is "ChangeRequested", "Modified", or "Draft"
            if (status != "ChangeRequested" && status != "Modified" && status != "Draft")
            {
                throw new InvalidOperationException(
                    $"Cannot update draft clause. Contract draft status must be one of: 'ChangeRequested', 'Modified', or 'Draft'.");
            }
        }
        else
        {
            throw new InvalidOperationException("Contract draft ID is required to update draft clause.");
        }

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
        
        // Bug Fix: Update BodyJson of contract draft when draft clause is updated
        if (contentChanged && contractDraftId.HasValue)
        {
            try
            {
                await _contractDraftsService.UpdateBodyJsonFromDraftClauseAsync(
                    contractDraftId.Value, 
                    updatedDraftClause.Id, 
                    updatedDraftClause.Title ?? string.Empty, 
                    updatedDraftClause.Body ?? string.Empty);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the update - the draft clause was already updated
                // In a production system, you'd want to log this properly
                System.Diagnostics.Debug.WriteLine($"Failed to update BodyJson from draft clause: {ex.Message}");
            }
        }
        
        return _mapper.Map<DraftClausesResponse>(updatedDraftClause);
    }

    public async Task<DraftClausesResponse> AddTemplateClauseToDraftAsync(int templateClauseId, int contractDraftId)
    {
        // Get the template clause
        var templateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == templateClauseId);
        if (templateClause == null)
        {
            throw new InvalidOperationException("Template clause not found.");
        }

        // Create a new draft clause from the template
        var draftClause = new DraftClauses
        {
            Title = templateClause.Title,
            Body = templateClause.Body,
            IsModified = false,  // Fresh copy from template, not modified yet
            ContractDraftsId = contractDraftId,
            TemplateClausesId = templateClause.Id,
            CreatedAt = DateTime.UtcNow
        };

        var createdDraftClause = await _draftClausesRepository.AddAsync(draftClause);
        
        return _mapper.Map<DraftClausesResponse>(createdDraftClause);
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

