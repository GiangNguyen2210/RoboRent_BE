using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IDraftClausesRepository : IGenericRepository<DraftClauses>
{
    Task<IEnumerable<DraftClauses>> GetDraftClausesByContractDraftIdAsync(int contractDraftId);
    Task<IEnumerable<DraftClauses>> GetDraftClausesByTemplateClauseIdAsync(int templateClauseId);
    Task<IEnumerable<DraftClauses>> GetAllWithIncludesAsync();
}
