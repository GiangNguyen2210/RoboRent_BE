using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.DTOs.ChecklistDeliveryEvidence;

public class ChecklistDeliveryEvidenceResponse
{
    public int Id { get; set; }

    public int? ChecklistDeliveryId { get; set; }

    public int? ChecklistDeliveryItemId { get; set; }

    public EvidenceScope Scope { get; set; } = EvidenceScope.Item;
    public EvidenceType Type { get; set; } = EvidenceType.Photo;

    public string Url { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public long? FileSizeBytes { get; set; }

    public DateTime? CapturedAt { get; set; }

    public int UploadedByStaffId { get; set; }

    public string? MetaJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}