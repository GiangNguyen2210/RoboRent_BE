using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class ChecklistDeliveryItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ChecklistDeliveryId { get; set; }
    [ForeignKey("ChecklistDeliveryId")]
    public ChecklistDelivery ChecklistDelivery { get; set; } = null!;

    // Item identity
    [MaxLength(80)]
    public string Key { get; set; } = string.Empty; // battery_level, emergency_stop...

    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // Power/Safety/A-V/Packaging...

    public ChecklistSeverity Severity { get; set; } = ChecklistSeverity.Medium;

    // Rules
    public bool IsRequired { get; set; } = true;
    public bool EvidenceRequiredOnFail { get; set; } = true;
    public bool MustPassToDispatch { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    [MaxLength(500)]
    public string? Expected { get; set; } // ">= 80%"

    [MaxLength(30)]
    public string ValueType { get; set; } = "select"; // bool/number/text/select...

    // values (generic)
    public bool? ValueBool { get; set; }
    public decimal? ValueNumber { get; set; }

    [MaxLength(2000)]
    public string? ValueText { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ValueJson { get; set; }

    public ChecklistItemResult Result { get; set; } = ChecklistItemResult.Unknown;

    [MaxLength(2000)]
    public string? Note { get; set; }

    // // audit per item (optional)
    // public DateTime? CheckedAt { get; set; }
    // public int? CheckedByStaffId { get; set; }
    // public Account? CheckedByStaff { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ChecklistDeliveryEvidence> Evidences { get; set; } = new List<ChecklistDeliveryEvidence>();
}
