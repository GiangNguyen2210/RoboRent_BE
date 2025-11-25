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
}