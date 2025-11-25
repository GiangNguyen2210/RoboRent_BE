using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class PriceQuoteRepository : GenericRepository<PriceQuote>, IPriceQuoteRepository
{
    public PriceQuoteRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<List<PriceQuote>> GetAllWithDetailsAsync(string? status = null)
    {
        var query = DbSet
            .Include(pq => pq.Rental)
            .ThenInclude(r => r.Account)
            .Include(pq => pq.Rental)
            .ThenInclude(r => r.ActivityType)           // ✅ FIX
            .Include(pq => pq.Rental)
            .ThenInclude(r => r.EventActivity)          // ✅ FIX
            .Where(pq => pq.IsDeleted != true);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(pq => pq.Status == status);
        }

        return await query
            .OrderByDescending(pq => pq.CreatedAt)
            .ToListAsync();
    }
}