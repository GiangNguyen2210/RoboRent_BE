using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class EventRoboTypeRepository : GenericRepository<EventRoboType>, IEventRoboTypeRepository
{
    public EventRoboTypeRepository(AppDbContext context) : base(context)
    {
    }
}