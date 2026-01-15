using RoboRent_BE.Model.DTOs.ChecklistDelivery;

namespace RoboRent_BE.Service.Interfaces;

public interface IChecklistDeliveryService
{
    public Task<ChecklistDeliveryResponse?> CreateChecklistDeliveryAsync(ChecklistDeliveryRequest request);
    
    public Task<dynamic>  UpdateChecklistDeliveryAsync(ChecklistUpdateDeliveryRequest request);

    public Task<ChecklistDeliveryResponse?> GetChecklistDeliveryByActAsync(int actId);
    
    public Task<ChecklistDeliveryResponse?> CustomerConfirmChecklistDelivery(int checklistDeliveryId, CustomerConfirmRequest customerConfirmRequest);
    
    public Task<int?> CustomerGetChecklistDeliveryByRentalIdAsync(int rentalId);
}