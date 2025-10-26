using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class TemplateClausesRepository : GenericRepository<TemplateClauses>, ITemplateClausesRepository
{
    private readonly AppDbContext _dbContext;
    public TemplateClausesRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<TemplateClauses>> GetTemplateClausesByContractTemplateIdAsync(int contractTemplateId)
    {
        return await _dbContext.TemplateClauses
            .Where(tc => tc.ContractTemplatesId == contractTemplateId)
            .Include(tc => tc.ContractTemplate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TemplateClauses>> GetTemplateClausesByIsMandatoryAsync(bool isMandatory)
    {
        return await _dbContext.TemplateClauses
            .Where(tc => tc.IsMandatory == isMandatory)
            .Include(tc => tc.ContractTemplate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TemplateClauses>> GetTemplateClausesByIsEditableAsync(bool isEditable)
    {
        return await _dbContext.TemplateClauses
            .Where(tc => tc.IsEditable == isEditable)
            .Include(tc => tc.ContractTemplate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TemplateClauses>> GetAllWithIncludesAsync()
    {
        return await _dbContext.TemplateClauses
            .Include(tc => tc.ContractTemplate)
            .ToListAsync();
    }
}
