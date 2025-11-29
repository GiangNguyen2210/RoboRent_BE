using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;


public class ActualDeliveryRepository : GenericRepository<ActualDelivery>, IActualDeliveryRepository
{
    private readonly AppDbContext _dbContext;

    public ActualDeliveryRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<ActualDelivery?> GetByRentalIdAsync(int rentalId)
    {
        return await DbSet
            .Include(d => d.Rental)
            .ThenInclude(r => r.Account)
            .Include(d => d.Staff)
            .FirstOrDefaultAsync(d => d.RentalId == rentalId);
    }

    public async Task<List<ActualDelivery>> GetByStaffIdAsync(int staffId)
    {
        return await DbSet
            .Include(d => d.Rental)
            .ThenInclude(r => r.Account)
            .Where(d => d.StaffId == staffId)
            .OrderBy(d => d.ScheduledDeliveryTime)
            .ToListAsync();
    }

    public async Task<List<ActualDelivery>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await DbSet
            .Include(d => d.Rental)
            .ThenInclude(r => r.Account)
            .Include(d => d.Staff)
            .Where(d => d.ScheduledDeliveryTime >= from && d.ScheduledDeliveryTime <= to)
            .OrderBy(d => d.ScheduledDeliveryTime)
            .ToListAsync();
    }
}