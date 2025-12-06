using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RoboTypeRepository : GenericRepository<RoboType>, IRoboTypeRepository
{
    private readonly AppDbContext _dbContext;
    public RoboTypeRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }
}