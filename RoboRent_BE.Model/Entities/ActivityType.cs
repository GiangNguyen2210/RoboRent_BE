using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class ActivityType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty; // BASIC / STANDARD / PREMIUM

    public string Name { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Bạn có thể giữ Price làm "giá tham chiếu/starting from"
    public decimal Price { get; set; } = 0m;

    public string Currency { get; set; } = "VND";

    public bool IncludesOperator { get; set; } = true;

    public int OperatorCount { get; set; } = 1;

    // ===== Option A pricing fields =====

    /// <summary>
    /// Đơn giá thuê robot theo giờ (không bao gồm phí nhân sự kỹ thuật).
    /// </summary>
    public decimal HourlyRate { get; set; } = 0m;

    /// <summary>
    /// Số phút tối thiểu tính tiền (vd 120 = 2 giờ).
    /// </summary>
    public int MinimumMinutes { get; set; } = 120;

    /// <summary>
    /// Bước làm tròn thời lượng tính tiền (vd 30 phút). Thời lượng sẽ được làm tròn lên.
    /// </summary>
    public int BillingIncrementMinutes { get; set; } = 30;

    /// <summary>
    /// Phí thuê 1 kỹ thuật viên theo giờ (nếu IncludesOperator = true).
    /// Total staff fee = TechnicalStaffFeePerHour * OperatorCount * BillableHours
    /// </summary>
    public decimal TechnicalStaffFeePerHour { get; set; } = 0m;

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
    
    public decimal DamageDeposit { get; set; } = 0m;

    public virtual ICollection<ActivityTypeGroup> ActivityTypeGroups { get; set; } = new List<ActivityTypeGroup>();
    public virtual ICollection<RobotTypeOfActivity> RobotTypeOfActivities { get; set; } = new List<RobotTypeOfActivity>();
}