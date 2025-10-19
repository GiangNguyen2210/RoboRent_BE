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
}