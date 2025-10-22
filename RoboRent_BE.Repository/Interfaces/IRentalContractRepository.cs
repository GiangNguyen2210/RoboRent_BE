using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IRentalContractRepository : IGenericRepository<RentalContract>
{
    Task<IEnumerable<RentalContract>> GetAllWithIncludesAsync();
    Task<IEnumerable<RentalContract>> GetRentalContractsByRentalIdAsync(int rentalId);
}
