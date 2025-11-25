using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RentalContractRepository : GenericRepository<RentalContract>, IRentalContractRepository
{
    public RentalContractRepository(AppDbContext db) : base(db)
    {
    }

    public async Task<IEnumerable<RentalContract>> GetAllWithIncludesAsync()
    {
        return await GetAllAsync(filter: rc => rc.IsDeleted != true, includeProperties: "Rental");
    }

    public async Task<IEnumerable<RentalContract>> GetRentalContractsByRentalIdAsync(int rentalId)
    {
        return await GetAllAsync(filter: rc => rc.RentalId == rentalId && rc.IsDeleted != true, includeProperties: "Rental");
    }
}
