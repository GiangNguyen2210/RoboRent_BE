using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RentalRepository : GenericRepository<Rental>, IRentalRepository
{
    public RentalRepository(AppDbContext context) : base(context)
    {
    }
}