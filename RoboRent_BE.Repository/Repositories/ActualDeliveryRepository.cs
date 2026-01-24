using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ActualDeliveryRepository : GenericRepository<ActualDelivery>, IActualDeliveryRepository
{
    public ActualDeliveryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ActualDelivery?> GetByGroupScheduleIdAsync(int groupScheduleId)
    {
        return await DbSet
            .Include(d => d.GroupSchedule)
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.Account)
            .Include(d => d.GroupSchedule) // Chain another Include for ActivityType
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.ActivityType)
            .Include(d => d.Staff)
            .FirstOrDefaultAsync(d => d.GroupScheduleId == groupScheduleId);
    }

    public async Task<List<ActualDelivery>> GetByStaffIdAsync(int staffId)
    {
        return await DbSet
            .Include(d => d.GroupSchedule)
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.Account)
            .Include(d => d.GroupSchedule) // Chain another Include for ActivityType
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.ActivityType)
            .Include(d => d.Staff)
            .Where(d => d.StaffId == staffId)
            .OrderBy(d => d.GroupSchedule.EventDate)
            // .ThenBy(d => d.GroupSchedule.DeliveryTime)
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
            // .ThenBy(d => d.GroupSchedule.DeliveryTime)
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
            .Include(d => d.GroupSchedule) // Chain another Include for ActivityType
                .ThenInclude(gs => gs.Rental)
                .ThenInclude(r => r.ActivityType)
            .Include(d => d.GroupSchedule.ActivityTypeGroup)
            .Include(d => d.Staff)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    
    public async Task<(List<ActualDelivery> items, int totalCount)> GetPendingDeliveriesAsync(
    int page, 
    int pageSize, 
    string? searchTerm = null, 
    string? sortBy = "date")
{
    // Base query: Status = Pending AND StaffId is null (chưa assign)
    var query = DbSet
        .Include(d => d.GroupSchedule)
            .ThenInclude(gs => gs.Rental)
            .ThenInclude(r => r.Account)
        .Include(d => d.Staff)
        .Where(d => d.Status == "Pending" && d.StaffId == null);

    // Search filter
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        var lowerSearch = searchTerm.ToLower();
        query = query.Where(d =>
            (d.GroupSchedule.Rental != null && 
             d.GroupSchedule.Rental.EventName != null && 
             d.GroupSchedule.Rental.EventName.ToLower().Contains(lowerSearch)) ||
            (d.GroupSchedule.EventLocation != null && 
             d.GroupSchedule.EventLocation.ToLower().Contains(lowerSearch)) ||
            (d.GroupSchedule.EventCity != null && 
             d.GroupSchedule.EventCity.ToLower().Contains(lowerSearch)) ||
            (d.GroupSchedule.Rental != null && 
             d.GroupSchedule.Rental.Account != null && 
             d.GroupSchedule.Rental.Account.FullName != null && 
             d.GroupSchedule.Rental.Account.FullName.ToLower().Contains(lowerSearch))
        );
    }

    // Get total count before pagination
    var totalCount = await query.CountAsync();

    // Sort
    query = sortBy?.ToLower() switch
    {
        "name" => query.OrderBy(d => d.GroupSchedule.Rental!.EventName),
        "customer" => query.OrderBy(d => d.GroupSchedule.Rental!.Account!.FullName),
        "location" => query.OrderBy(d => d.GroupSchedule.EventLocation),
        _ => query.OrderBy(d => d.GroupSchedule.EventDate)
                    // .ThenBy(d => d.GroupSchedule.DeliveryTime) // Sort by time nếu cùng ngày
    };

    // Pagination
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (items, totalCount);
}
}