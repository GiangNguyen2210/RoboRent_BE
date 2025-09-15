using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RentalDetailRepository : GenericRepository<RentalDetail>, IRentalDetailRepository
{
    public RentalDetailRepository(AppDbContext context) : base(context)
    {
    }
}