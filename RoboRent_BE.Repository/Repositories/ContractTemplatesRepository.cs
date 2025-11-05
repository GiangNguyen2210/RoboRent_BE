using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ContractTemplatesRepository : GenericRepository<ContractTemplates>, IContractTemplatesRepository
{
    private readonly AppDbContext _dbContext;
    public ContractTemplatesRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<ContractTemplates>> GetContractTemplatesByStatusAsync(string status)
    {
        return await _dbContext.ContractTemplates
            .Where(ct => ct.Status == status)
            .Include(ct => ct.Created)
            .Include(ct => ct.Updated)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractTemplates>> GetContractTemplatesByCreatedByAsync(int createdBy)
    {
        return await _dbContext.ContractTemplates
            .Where(ct => ct.CreatedBy == createdBy)
            .Include(ct => ct.Created)
            .Include(ct => ct.Updated)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractTemplates>> GetContractTemplatesByVersionAsync(string version)
    {
        return await _dbContext.ContractTemplates
            .Where(ct => ct.Version == version)
            .Include(ct => ct.Created)
            .Include(ct => ct.Updated)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractTemplates>> GetAllWithIncludesAsync()
    {
        return await _dbContext.ContractTemplates
            .Include(ct => ct.Created)
            .Include(ct => ct.Updated)
            .ToListAsync();
    }
}

