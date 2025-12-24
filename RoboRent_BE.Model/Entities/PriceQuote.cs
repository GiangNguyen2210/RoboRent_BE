using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class PriceQuote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int RentalId { get; set; }

    [ForeignKey(nameof(RentalId))]
    public virtual Rental Rental { get; set; } = null!;

    // === DEPOSIT COMPONENTS (Auto-calculated, LOCKED after Phase 1) ===

    /// <summary>
    /// Phí thuê robot = HourlyRate × BillableHours (từ ActivityType)
    /// </summary>
    public decimal RentalFee { get; set; } = 0m;

    /// <summary>
    /// Phí nhân viên kỹ thuật = TechnicalStaffFeePerHour × OperatorCount × BillableHours
    /// </summary>
    public decimal StaffFee { get; set; } = 0m;

    /// <summary>
    /// Tiền cọc thiệt hại (từ ActivityType.DamageDeposit)
    /// </summary>
    public decimal DamageDeposit { get; set; } = 0m;

    // === ADJUSTABLE FEES (Can be modified in Phase 2) ===

    /// <summary>
    /// Phí giao hàng (auto-calculated theo city, có thể sửa nếu địa chỉ thay đổi)
    /// </summary>
    public decimal? DeliveryFee { get; set; }

    /// <summary>
    /// Khoảng cách giao hàng (km)
    /// </summary>
    public int? DeliveryDistance { get; set; }

    /// <summary>
    /// Phí cấu hình abilities (từ IsPriceImpacting abilities)
    /// Thay thế field Service cũ
    /// </summary>
    public decimal CustomizationFee { get; set; } = 0m;

    // === COMPUTED TOTALS (Not stored, calculated) ===

    /// <summary>
    /// Tổng tiền cọc = 30% × (RentalFee + StaffFee) + DamageDeposit
    /// Khách trả trước khi ký contract
    /// </summary>
    [NotMapped]
    public decimal TotalDeposit => 0.3m * (RentalFee + StaffFee) + DamageDeposit;

    /// <summary>
    /// Tổng tiền thanh toán sau = 70% × (RentalFee + StaffFee) + DeliveryFee + CustomizationFee
    /// Khách trả sau khi hoàn thành
    /// </summary>
    [NotMapped]
    public decimal TotalPayment => 0.7m * (RentalFee + StaffFee) + (DeliveryFee ?? 0) + CustomizationFee;

    /// <summary>
    /// Tổng giá trị đơn hàng
    /// </summary>
    [NotMapped]
    public decimal GrandTotal => TotalDeposit + TotalPayment;

    // === METADATA ===

    public string? StaffDescription { get; set; }
    public string? ManagerFeedback { get; set; }
    public string? CustomerReason { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public bool? IsDeleted { get; set; } = false;
    public string? Status { get; set; } = string.Empty;

    public int? ManagerId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public virtual Account? Manager { get; set; }

    public DateTime? SubmittedToManagerAt { get; set; }
    public DateTime? ManagerApprovedAt { get; set; }
}