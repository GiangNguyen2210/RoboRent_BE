using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class PriceQuoteRepository : GenericRepository<PriceQuote>, IPriceQuoteRepository
{
    private readonly AppDbContext _dbContext;

    public PriceQuoteRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<int> CountByRentalIdAsync(int rentalId)
    {
        return await DbSet
            .Where(pq => pq.RentalId == rentalId && pq.IsDeleted != true)
            .CountAsync();
    }

    public async Task<List<PriceQuote>> GetByRentalIdAsync(int rentalId)
    {
        return await DbSet
            .Where(pq => pq.RentalId == rentalId && pq.IsDeleted != true)
            .OrderBy(pq => pq.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<List<PriceQuote>> GetAllWithDetailsAsync(string? status = null)
    {
        var query = DbSet
            .Include(pq => pq.Rental)
            .ThenInclude(r => r.Account)
            .Include(pq => pq.Rental)
            .ThenInclude(r => r.RentalPackage)
            .Include(pq => pq.Rental)
            .ThenInclude(r => r.EventSchedules) // ✅ Giống ChatService
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