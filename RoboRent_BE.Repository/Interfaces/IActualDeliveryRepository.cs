using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IActualDeliveryRepository : IGenericRepository<ActualDelivery>
{
    /// <summary>
    /// Get ActualDelivery by GroupScheduleId
    /// </summary>
    Task<ActualDelivery?> GetByGroupScheduleIdAsync(int groupScheduleId);
    
    /// <summary>
    /// Get all deliveries of a staff
    /// </summary>
    Task<List<ActualDelivery>> GetByStaffIdAsync(int staffId);
    
    /// <summary>
    /// Get deliveries by date range
    /// </summary>
    Task<List<ActualDelivery>> GetByDateRangeAsync(DateTime from, DateTime to);
    
    /// <summary>
    /// Get deliveries by staff + date range (for conflict checking)
    /// </summary>
    Task<List<ActualDelivery>> GetByStaffAndDateRangeAsync(int staffId, DateTime from, DateTime to);
    
    /// <summary>
    /// Get delivery with full navigation properties
    /// </summary>
    Task<ActualDelivery?> GetWithDetailsAsync(int id);
    
    /// <summary>
    /// Get pending deliveries with pagination, search, and sort
    /// </summary>
    Task<(List<ActualDelivery> items, int totalCount)> GetPendingDeliveriesAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? sortBy = "date");
}