using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class EventActivityRepository:  GenericRepository<EventActivity>, IEventActivityRepository
{
    private readonly AppDbContext _dbContext;
    
    public EventActivityRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}