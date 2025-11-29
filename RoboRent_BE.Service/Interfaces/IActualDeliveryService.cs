using RoboRent_BE.Model.DTOs.ActualDelivery;

namespace RoboRent_BE.Service.Interfaces;

public interface IActualDeliveryService
{
    Task<ActualDeliveryResponse> CreateActualDeliveryAsync(CreateActualDeliveryRequest request);
    Task<ActualDeliveryResponse> AssignDeliveryAsync(int deliveryId, AssignDeliveryRequest request, int staffId);
    Task<ActualDeliveryResponse> UpdateStatusAsync(int deliveryId, UpdateDeliveryStatusRequest request, int staffId);
    Task<ActualDeliveryResponse> GetByIdAsync(int id);
    Task<ActualDeliveryResponse> GetByRentalIdAsync(int rentalId);
    Task<List<ActualDeliveryResponse>> GetByStaffIdAsync(int staffId);
    Task<List<DeliveryCalendarResponse>> GetCalendarAsync(DateTime from, DateTime to, int? staffId = null);
    
    /// <summary>
    /// [MANAGER] Check conflict khi assign staff
    /// </summary>
    Task<ConflictCheckResponse> CheckStaffConflictAsync(int staffId, int groupScheduleId);
}