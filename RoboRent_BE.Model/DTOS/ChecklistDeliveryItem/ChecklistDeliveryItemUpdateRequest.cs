using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.DTOs.ChecklistDeliveryItem;

public class ChecklistDeliveryItemUpdateRequest
{
    public int Id { get; set; }

    public string ValueType { get; set; } = "select"; // bool/number/text/select...

    public bool? ValueBool { get; set; }
    public decimal? ValueNumber { get; set; }

    public string? ValueText { get; set; }

    public string? ValueJson { get; set; }
    
    public string? Note { get; set; }
    
    public ChecklistItemResult Result { get; set; } = ChecklistItemResult.Unknown;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}