using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ActivityTypeGroupRepository : GenericRepository<ActivityTypeGroup>, IActivityTypeGroupRepository
{
    private readonly AppDbContext _dbContext;
    public ActivityTypeGroupRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext =  dbContext;
    }
}