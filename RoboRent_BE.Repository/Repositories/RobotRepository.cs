using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RobotRepository : GenericRepository<Robot>, IRobotRepository
{
    public RobotRepository(AppDbContext context) : base(context)
    {
    }
}