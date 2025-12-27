using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RobotAbilityValueRepository : GenericRepository<RobotAbilityValue>, IRobotAbilityValueRepository
{
    public RobotAbilityValueRepository(AppDbContext db) : base(db)
    {
    }
}