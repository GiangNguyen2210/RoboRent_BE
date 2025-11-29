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
            .Include(d => d.GroupSchedule)
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.Account)
            .Include(d => d.Staff)
            .Where(d => d.GroupSchedule.EventDate >= from.Date && d.GroupSchedule.EventDate <= to.Date)
            .OrderBy(d => d.GroupSchedule.EventDate)
            .ThenBy(d => d.GroupSchedule.DeliveryTime)
            .ToListAsync();
    }

    public async Task<List<ActualDelivery>> GetByStaffAndDateRangeAsync(int staffId, DateTime from, DateTime to)
    {
        return await DbSet
            .Include(d => d.GroupSchedule)
            .Where(d => d.StaffId == staffId 
                && d.GroupSchedule.EventDate >= from.Date 
                && d.GroupSchedule.EventDate <= to.Date)
            .ToListAsync();
    }

    public async Task<ActualDelivery?> GetWithDetailsAsync(int id)
    {
        return await DbSet
            .Include(d => d.GroupSchedule)
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.Account)
            .Include(d => d.GroupSchedule.ActivityTypeGroup)
            .Include(d => d.Staff)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}