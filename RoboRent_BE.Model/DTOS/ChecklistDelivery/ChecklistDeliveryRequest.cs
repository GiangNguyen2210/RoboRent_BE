using System.ComponentModel;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.DTOs.ChecklistDelivery;

public class ChecklistDeliveryRequest
{

    // FK: gắn vào chuyến giao
    public int ActualDeliveryId { get; set; }
    
    [DefaultValue(ChecklistDeliveryType.PreDispatch)]
    public ChecklistDeliveryType Type { get; set; } = ChecklistDeliveryType.PreDispatch;
    
    [DefaultValue(ChecklistDeliveryStatus.Draft)]
    public ChecklistDeliveryStatus Status { get; set; } = ChecklistDeliveryStatus.Draft;
    
    // Kết quả tổng
    [DefaultValue(ChecklistItemResult.Unknown)]
    public ChecklistItemResult OverallResult { get; set; } = ChecklistItemResult.Unknown;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}