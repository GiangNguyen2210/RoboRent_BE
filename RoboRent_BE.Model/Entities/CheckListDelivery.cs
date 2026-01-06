using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class ChecklistDelivery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // FK: gắn vào chuyến giao
    public int ActualDeliveryId { get; set; }
    [ForeignKey("ActualDeliveryId")]
    public virtual ActualDelivery ActualDelivery { get; set; } = null!;
    
    [MaxLength(50)]
    public string? ChecklistNo { get; set; } = string.Empty; // CLD-YYYYMMDD-XXXX

    public ChecklistDeliveryType? Type { get; set; } = ChecklistDeliveryType.PreDispatch;
    public ChecklistDeliveryStatus? Status { get; set; } = ChecklistDeliveryStatus.Draft;

    // Người kiểm
    public int? CheckedByStaffId { get; set; }
    [ForeignKey("CheckedByStaffId")]
    public Account? CheckedByStaff { get; set; } = null!;
    public DateTime? CheckedAt { get; set; }

    // Phase 2
    public DateTime? CustomerAcceptedAt { get; set; }
    
    public int? CustomerAcceptedById { get; set; }
    [ForeignKey("CustomerAcceptedById")]
    public Account? CustomerAccepted { get; set; } = null!;
    
    public string? CustomerSignatureUrl { get; set; }
    public string? CustomerNote { get; set; }
    
    // // Người duyệt (optional)
    // public int? ApprovedByStaffId { get; set; }
    // public Account? ApprovedByStaff { get; set; }
    // public DateTime? ApprovedAt { get; set; }

    // Kết quả tổng
    public ChecklistItemResult? OverallResult { get; set; } = ChecklistItemResult.Unknown;

    [MaxLength(2000)]
    public string? OverallNote { get; set; }

    // Summary counts (để query nhanh)
    public int? TotalItems { get; set; }
    public int? PassItems { get; set; }
    public int? FailItems { get; set; }

    // // Optional: chữ ký / biên bản
    // [MaxLength(500)]
    // public string? SignatureUrl { get; set; }

    // context snapshot (jsonb): robotIds, packageCode, event info...
    [Column(TypeName = "jsonb")]
    public string? MetaJson { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ChecklistDeliveryItem>? Items { get; set; } = new List<ChecklistDeliveryItem>();
    public ICollection<ChecklistDeliveryEvidence>? Evidences { get; set; } = new List<ChecklistDeliveryEvidence>();
    
    
}
