using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IPriceQuoteRepository : IGenericRepository<PriceQuote>
{
    Task<int> CountByRentalIdAsync(int rentalId);
    Task<List<PriceQuote>> GetByRentalIdAsync(int rentalId);
    Task<List<PriceQuote>> GetAllWithDetailsAsync(string? status = null);
}