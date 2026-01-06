using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.DTOs.ChecklistDeliveryItem;

public class ChecklistDeliveryItemResponse
{
    public int Id { get; set; }

    public int ChecklistDeliveryId { get; set; }
    
    public string Key { get; set; } = string.Empty; // battery_level, emergency_stop...

    public string Label { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty; // Power/Safety/A-V/Packaging...

    public ChecklistSeverity Severity { get; set; } = ChecklistSeverity.Medium;

    // Rules
    public bool IsRequired { get; set; } = true;
    public bool EvidenceRequiredOnFail { get; set; } = true;
    public bool MustPassToDispatch { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    public string? Expected { get; set; } // ">= 80%"

    public string ValueType { get; set; } = "select"; // bool/number/text/select...

    // values (generic)
    public bool? ValueBool { get; set; }
    public decimal? ValueNumber { get; set; }

    public string? ValueText { get; set; }

    public string? ValueJson { get; set; }

    public ChecklistItemResult Result { get; set; } = ChecklistItemResult.Unknown;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}