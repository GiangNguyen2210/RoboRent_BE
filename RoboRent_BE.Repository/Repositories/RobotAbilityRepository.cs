using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RobotAbilityRepository : GenericRepository<RobotAbility>, IRobotAbilityRepository
{
    public RobotAbilityRepository(AppDbContext db) : base(db)
    {
    }
}