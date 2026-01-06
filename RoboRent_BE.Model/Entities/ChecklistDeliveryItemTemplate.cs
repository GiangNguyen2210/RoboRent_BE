using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

/// <summary>
/// Template cho các item checklist (robot-centric).
/// Mỗi lần tạo ChecklistDelivery sẽ clone các template phù hợp ra ChecklistDeliveryItem.
/// </summary>
public class ChecklistDeliveryItemTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int? RoboTypeId { get; set; }
    [ForeignKey("RoboTypeId")]
    public RoboType? RoboType { get; set; } = null!;

    /// <summary>
    /// Mã duy nhất, dùng ổn định để mapping và seed.
    /// VD: APPEARANCE_SCRATCH, BATTERY_LEVEL, SPEAKER_TEST
    /// </summary>
    [Required]
    [MaxLength(80)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nhóm hiển thị cho UI (Appearance/Power/Audio...).
    /// </summary>
    [Required]
    public ChecklistItemGroup Group { get; set; } = ChecklistItemGroup.General;

    /// <summary>
    /// Tiêu đề ngắn gọn để staff/khách đọc nhanh.
    /// </summary>
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả ngắn hướng dẫn cách kiểm tra.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Thứ tự hiển thị trong nhóm.
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Item quan trọng: thường show ưu tiên cho khách xem tại hiện trường.
    /// </summary>
    public bool IsCritical { get; set; } = false;

    /// <summary>
    /// Có yêu cầu nhập giá trị đo/ghi nhận không (pin %, version, serial...).
    /// </summary>
    public bool RequiresMeasuredValue { get; set; } = false;

    /// <summary>
    /// Gợi ý label cho measured value. VD: "Battery (%)", "Firmware Version", "Serial No"
    /// </summary>
    [MaxLength(60)]
    public string? MeasuredValueLabel { get; set; }

    /// <summary>
    /// Quy định loại bằng chứng cần đính kèm cho item này.
    /// </summary>
    public EvidenceRequirement EvidenceRequirement { get; set; } = EvidenceRequirement.None;

    /// <summary>
    /// Nếu Fail thì bắt buộc note.
    /// </summary>
    public bool FailRequiresNote { get; set; } = true;

    /// <summary>
    /// Nếu Fail thì bắt buộc có evidence (ảnh/video).
    /// </summary>
    public bool FailRequiresEvidence { get; set; } = true;

    /// <summary>
    /// Template áp dụng theo RobotType (nếu null: dùng chung cho mọi robot).
    /// </summary>
    public int? RobotTypeId { get; set; }

    /// <summary>
    /// Template áp dụng theo ActivityType/Package (nếu null: dùng chung).
    /// </summary>
    public int? ActivityTypeId { get; set; }

    /// <summary>
    /// Bật/tắt template (để retire item cũ mà không mất lịch sử).
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Nhóm checklist item để UI gom cụm.
/// </summary>
public enum ChecklistItemGroup
{
    General = 0,
    Appearance = 1,
    Power = 2,
    Mobility = 3,
    Audio = 4,
    Display = 5,
    Sensors = 6,
    Accessories = 7,
    SafetySeal = 8,
    Documents = 9
}

/// <summary>
/// Quy định bằng chứng (ảnh/video) khi kiểm tra.
/// </summary>
public enum EvidenceRequirement
{
    None = 0,
    Photo = 1,
    Video = 2,
    Any = 3
}
