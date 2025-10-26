using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class DraftClausesRepository : GenericRepository<DraftClauses>, IDraftClausesRepository
{
    private readonly AppDbContext _dbContext;
    public DraftClausesRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<DraftClauses>> GetDraftClausesByContractDraftIdAsync(int contractDraftId)
    {
        return await _dbContext.DraftClauses
            .Where(dc => dc.ContractDraftsId == contractDraftId)
            .Include(dc => dc.ContractDraft)
            .Include(dc => dc.TemplateClause)
            .ToListAsync();
    }

    public async Task<IEnumerable<DraftClauses>> GetDraftClausesByTemplateClauseIdAsync(int templateClauseId)
    {
        return await _dbContext.DraftClauses
            .Where(dc => dc.TemplateClausesId == templateClauseId)
            .Include(dc => dc.ContractDraft)
            .Include(dc => dc.TemplateClause)
            .ToListAsync();
    }

    public async Task<IEnumerable<DraftClauses>> GetAllWithIncludesAsync()
    {
        return await _dbContext.DraftClauses
            .Include(dc => dc.ContractDraft)
            .Include(dc => dc.TemplateClause)
            .ToListAsync();
    }
}
