using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IRentalDetailRepository : IGenericRepository<RentalDetail>
{
    Task<IEnumerable<RentalDetail>> GetRentalDetailsByRentalIdAsync(int rentalId);
    Task<IEnumerable<RentalDetail>> GetRentalDetailsByRoboTypeIdAsync(int roboTypeId);
    Task<IEnumerable<RentalDetail>> GetAllWithIncludesAsync();
}