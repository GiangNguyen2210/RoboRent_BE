using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class EventScheduleRepository : GenericRepository<EventSchedule>, IEventScheduleRepository
{
    public EventScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<EventSchedule?> GetByIdAsync(int id)
    {
        return await DbSet.FirstOrDefaultAsync(es => es.Id == id && es.IsDeleted != true);
    }

    public async Task<List<EventSchedule>> GetByRentalIdAsync(int rentalId)
    {
        return await DbSet
            .Where(es => es.RentalId == rentalId && es.IsDeleted != true)
            .OrderBy(es => es.Date)
            .ThenBy(es => es.StartTime)
            .ToListAsync();
    }
}