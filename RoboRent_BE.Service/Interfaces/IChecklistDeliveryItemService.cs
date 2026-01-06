using RoboRent_BE.Model.DTOs.ChecklistDeliveryItem;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IChecklistDeliveryItemService
{
    public Task<List<ChecklistDeliveryItemResponse>?> CreateItemAsync(int checklistDeliveryId);

    public Task<List<ChecklistDeliveryItemResponse>?> GetAllChecklistDeliveryItemsAsync(int checklistDeliveryId);
}