using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context)
    {
    }
}