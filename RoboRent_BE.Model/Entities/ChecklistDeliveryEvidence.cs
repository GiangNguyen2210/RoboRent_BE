using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class ChecklistDeliveryEvidence
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? ChecklistDeliveryId { get; set; }
    [ForeignKey("ChecklistDeliveryId")]
    public ChecklistDelivery? ChecklistDelivery { get; set; } = null!;

    // Optional: evidence thuộc item nào
    public int? ChecklistDeliveryItemId { get; set; }

    [ForeignKey("ChecklistDeliveryItemId")]
    public ChecklistDeliveryItem? ChecklistDeliveryItem { get; set; } = null!;

    public EvidenceScope Scope { get; set; } = EvidenceScope.Item;
    public EvidenceType Type { get; set; } = EvidenceType.Photo;

    [MaxLength(1000)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? FileName { get; set; }

    public long? FileSizeBytes { get; set; }

    public DateTime? CapturedAt { get; set; }

    public int UploadedByStaffId { get; set; }
    [ForeignKey("UploadedByStaffId")]
    public Account UploadedByStaff { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string? MetaJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}