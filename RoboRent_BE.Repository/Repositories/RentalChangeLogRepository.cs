using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RentalChangeLogRepository : GenericRepository<RentalChangeLog>, IRentalChangeLogRepository
{
    public RentalChangeLogRepository(AppDbContext db) : base(db)
    {
        
    }
}