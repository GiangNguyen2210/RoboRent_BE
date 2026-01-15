using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Model.DTOs.ChecklistDelivery;

public class ChecklistDeliveryResponse
{
    public int Id { get; set; }

    // FK: gắn vào chuyến giao
    public int ActualDeliveryId { get; set; }
    
    public string ChecklistNo { get; set; } = string.Empty; // CLD-YYYYMMDD-XXXX

    public ChecklistDeliveryType Type { get; set; } = ChecklistDeliveryType.PreDispatch;
    public ChecklistDeliveryStatus Status { get; set; } = ChecklistDeliveryStatus.Draft;

    // Người kiểm
    public int CheckedByStaffId { get; set; }
    public DateTime? CheckedAt { get; set; }

    // Phase 2
    public DateTime? CustomerAcceptedAt { get; set; }
    
    public int? CustomerAcceptedById { get; set; }
    
    public string? CustomerSignatureUrl { get; set; }
    public string? CustomerNote { get; set; }
    
    // // Người duyệt (optional)
    // public int? ApprovedByStaffId { get; set; }
    // public Account? ApprovedByStaff { get; set; }
    // public DateTime? ApprovedAt { get; set; }

    // Kết quả tổng
    public ChecklistItemResult OverallResult { get; set; } = ChecklistItemResult.Unknown;

    public string? OverallNote { get; set; }

    // Summary counts (để query nhanh)
    public int TotalItems { get; set; }
    public int PassItems { get; set; }
    public int FailItems { get; set; }
    
    public string? MetaJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}