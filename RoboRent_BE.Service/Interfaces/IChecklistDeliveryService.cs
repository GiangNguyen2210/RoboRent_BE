using RoboRent_BE.Model.DTOs.ChecklistDelivery;

namespace RoboRent_BE.Service.Interfaces;

public interface IChecklistDeliveryService
{
    public Task<ChecklistDeliveryResponse?> CreateChecklistDeliveryAsync(ChecklistDeliveryRequest request);
    
    public Task<dynamic>  UpdateChecklistDeliveryAsync(ChecklistUpdateDeliveryRequest request);
}