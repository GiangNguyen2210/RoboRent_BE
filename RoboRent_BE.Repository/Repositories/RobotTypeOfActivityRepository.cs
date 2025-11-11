using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RobotTypeOfActivityRepository: GenericRepository<RobotTypeOfActivity>, IRobotTypeOfActivityRepository
{
    private readonly AppDbContext _dbContext;

    public RobotTypeOfActivityRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}