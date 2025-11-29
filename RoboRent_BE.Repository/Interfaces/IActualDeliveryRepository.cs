using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IActualDeliveryRepository : IGenericRepository<ActualDelivery>
{
    Task<ActualDelivery?> GetByRentalIdAsync(int rentalId);
    Task<List<ActualDelivery>> GetByStaffIdAsync(int staffId);
    Task<List<ActualDelivery>> GetByDateRangeAsync(DateTime from, DateTime to);
}