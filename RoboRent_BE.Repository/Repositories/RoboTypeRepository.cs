using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RoboTypeRepository : GenericRepository<RoboType>, IRoboTypeRepository
{
    public RoboTypeRepository(AppDbContext context) : base(context)
    {
    }
}