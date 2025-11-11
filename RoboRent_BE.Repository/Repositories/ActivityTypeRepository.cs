using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ActivityTypeRepository : GenericRepository<ActivityType>, IActivityTypeRepository
{
    private readonly AppDbContext _dbContext;
    
    public ActivityTypeRepository(AppDbContext db) : base(db)
    {
        _dbContext = db;
    }
}