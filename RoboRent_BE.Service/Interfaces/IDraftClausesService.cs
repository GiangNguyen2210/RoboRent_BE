using RoboRent_BE.Model.DTOS.DraftClauses;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IDraftClausesService
{
    Task<IEnumerable<DraftClausesResponse>> GetAllDraftClausesAsync();
    Task<DraftClausesResponse?> GetDraftClausesByIdAsync(int id);
    Task<IEnumerable<DraftClausesResponse>> GetDraftClausesByContractDraftIdAsync(int contractDraftId);
    Task<IEnumerable<DraftClausesResponse>> GetDraftClausesByTemplateClauseIdAsync(int templateClauseId);
    Task<DraftClausesResponse> CreateDraftClausesAsync(CreateDraftClausesRequest request);
    Task<DraftClausesResponse?> UpdateDraftClausesAsync(UpdateDraftClausesRequest request);
    Task<bool> DeleteDraftClausesAsync(int id);
}
