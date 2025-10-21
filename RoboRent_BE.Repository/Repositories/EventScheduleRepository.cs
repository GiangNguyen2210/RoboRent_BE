using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class EventScheduleRepository : GenericRepository<EventSchedule>, IEventScheduleRepository
{
    public EventScheduleRepository(AppDbContext context) : base(context)
    {
    }
}