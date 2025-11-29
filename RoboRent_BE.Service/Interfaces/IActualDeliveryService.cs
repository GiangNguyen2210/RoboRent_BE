using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.DTOs.ActualDelivery;

namespace RoboRent_BE.Service.Interfaces;

public interface IActualDeliveryService
{
    /// <summary>
    /// [AUTO] Tạo ActualDelivery khi customer accept contract
    /// </summary>
    Task<ActualDeliveryResponse> CreateActualDeliveryAsync(CreateActualDeliveryRequest request);
    
    /// <summary>
    /// [MANAGER] Assign staff technical cho delivery
    /// </summary>
    Task<ActualDeliveryResponse> AssignStaffAsync(int deliveryId, AssignStaffRequest request);
    
    /// <summary>
    /// [STAFF] Update delivery status (progress tracking)
    /// </summary>
    Task<ActualDeliveryResponse> UpdateStatusAsync(int deliveryId, UpdateDeliveryStatusRequest request, int staffId);
    
    /// <summary>
    /// [STAFF] Update notes only
    /// </summary>
    Task<ActualDeliveryResponse> UpdateNotesAsync(int deliveryId, UpdateDeliveryNotesRequest request, int staffId);
    
    /// <summary>
    /// Get delivery by ID
    /// </summary>
    Task<ActualDeliveryResponse> GetByIdAsync(int id);
    
    /// <summary>
    /// Get delivery by GroupScheduleId
    /// </summary>
    Task<ActualDeliveryResponse?> GetByGroupScheduleIdAsync(int groupScheduleId);
    
    /// <summary>
    /// [STAFF] Get own deliveries
    /// </summary>
    Task<List<ActualDeliveryResponse>> GetByStaffIdAsync(int staffId);
    
    /// <summary>
    /// [MANAGER/STAFF] View calendar by date range
    /// </summary>
    Task<List<DeliveryCalendarResponse>> GetCalendarAsync(DateTime from, DateTime to, int? staffId = null);
    
    /// <summary>
    /// [MANAGER] Check conflict khi assign staff
    /// </summary>
    Task<ConflictCheckResponse> CheckStaffConflictAsync(int staffId, int groupScheduleId);
    
    /// <summary>
    /// Get pending deliveries for manager assignment
    /// </summary>
    Task<PageListResponse<ActualDeliveryResponse>> GetPendingDeliveriesAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? sortBy = "date");
}