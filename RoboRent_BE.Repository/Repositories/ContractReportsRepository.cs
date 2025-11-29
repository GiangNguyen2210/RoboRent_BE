using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ContractReportsRepository : GenericRepository<ContractReports>, IContractReportsRepository
{
    private readonly AppDbContext _dbContext;
    
    public ContractReportsRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<ContractReports>> GetPendingReportsAsync()
    {
        return await _dbContext.ContractReports
            .Where(cr => cr.Status == "Pending")
            .Include(cr => cr.DraftClauses)
            .Include(cr => cr.Account)
            .Include(cr => cr.Accused)
            .Include(cr => cr.Manager)
            // .Include(cr => cr.PaymentTransaction)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractReports>> GetReportsPendingExpirationAsync()
    {
        var threeDaysAgo = DateTime.UtcNow.AddDays(-3);
        return await _dbContext.ContractReports
            .Where(cr => cr.Status == "Pending" && cr.CreatedAt.HasValue && cr.CreatedAt.Value <= threeDaysAgo)
            .ToListAsync();
    }
}


