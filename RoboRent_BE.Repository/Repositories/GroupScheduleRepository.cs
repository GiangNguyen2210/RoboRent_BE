using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces; 

namespace RoboRent_BE.Repository.Repositories;

public class GroupScheduleRepository : GenericRepository<GroupSchedule>, IGroupScheduleRepository
{
    private readonly AppDbContext _dbContext;
    
    public GroupScheduleRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<GroupSchedule?> GetByRentalAsync(int rentalId)
    {
        return await _dbContext.GroupSchedules
            .FirstOrDefaultAsync(g => g.RentalId == rentalId);
    }

    public async Task<GroupSchedule?> GetByIdAsync(int id)
    {
        return await _dbContext.GroupSchedules.FindAsync(id);
    }

    public async Task<IEnumerable<GroupSchedule>> GetSameDaySchedulesAsync(int groupId, DateTime date)
    {
        return await _dbContext.GroupSchedules
            .Include(g => g.Rental)
            .Include(g => g.ActivityTypeGroup)
            .Where(g => g.ActivityTypeGroupId == groupId &&
                        g.EventDate == date &&
                        !g.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<GroupSchedule>> GetByEventDateAndActivityTypeGroupAsync(DateTime eventDate, int activityTypeGroupId)
    {
        return await _dbContext.GroupSchedules
            .Include(gs => gs.Rental)
            .Include(gs => gs.ActivityTypeGroup)
            .ThenInclude(atg => atg.ActivityType)
            .Where(gs =>
                gs.EventDate.HasValue &&
                gs.EventDate.Value.Date == eventDate.Date &&
                gs.ActivityTypeGroupId == activityTypeGroupId &&
                !gs.IsDeleted)
            .ToListAsync();
    }

    public async Task AddAsync(GroupSchedule entity)
    {
        await _dbContext.GroupSchedules.AddAsync(entity);
    }

    public async Task UpdateNotGenericAsync(GroupSchedule entity)
    {
        _dbContext.GroupSchedules.Update(entity);
    }

    public void Attach(GroupSchedule entity)
    {
        _dbContext.GroupSchedules.Attach(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}