using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IActualDeliveryRepository : IGenericRepository<ActualDelivery>
{
    Task<ActualDelivery?> GetByRentalIdAsync(int rentalId);
    Task<List<ActualDelivery>> GetByStaffIdAsync(int staffId);
    Task<List<ActualDelivery>> GetByDateRangeAsync(DateTime from, DateTime to);
    
    /// <summary>
    /// Get deliveries by staff + date range (for conflict checking)
    /// </summary>
    Task<List<ActualDelivery>> GetByStaffAndDateRangeAsync(int staffId, DateTime from, DateTime to);
    
    /// <summary>
    /// Get delivery with full navigation properties
    /// </summary>
    Task<ActualDelivery?> GetWithDetailsAsync(int id);
}