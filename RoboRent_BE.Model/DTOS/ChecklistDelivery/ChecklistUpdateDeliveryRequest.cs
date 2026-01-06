using RoboRent_BE.Model.DTOs.ChecklistDeliveryItem;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.DTOs.ChecklistDelivery;

public class ChecklistUpdateDeliveryRequest
{
    public int? checklistDeliveryId { get; set; }
    public string? ChecklistNo { get; set; } = string.Empty; // CLD-YYYYMMDD-XXXX

    public ChecklistDeliveryType? Type { get; set; } = ChecklistDeliveryType.PreDispatch;
    public ChecklistDeliveryStatus? Status { get; set; } = ChecklistDeliveryStatus.Draft;

    // Người kiểm
    public int? CheckedByStaffId { get; set; }

    public DateTime? CheckedAt { get; set; }
    
    public ChecklistItemResult? OverallResult { get; set; } = ChecklistItemResult.Unknown;

    public string? OverallNote { get; set; }

    // Summary counts (để query nhanh)
    public int? TotalItems { get; set; }
    public int? PassItems { get; set; }
    public int? FailItems { get; set; }
    
    // context snapshot (jsonb): robotIds, packageCode, event info...
    public string? MetaJson { get; set; }
    
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public List<ChecklistDeliveryItemUpdateRequest> ChecklistDeliveryItemUpdateRequests { get; set; } = new List<ChecklistDeliveryItemUpdateRequest>();
}