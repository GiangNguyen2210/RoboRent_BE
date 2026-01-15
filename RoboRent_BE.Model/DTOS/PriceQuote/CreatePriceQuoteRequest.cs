using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.PriceQuote;

/// <summary>
/// Request tạo báo giá mới
/// RentalFee, StaffFee, DamageDeposit sẽ được auto-calculated từ ActivityType
/// </summary>
public class CreatePriceQuoteRequest
{
    [Required]
    public int RentalId { get; set; }

    /// <summary>
    /// Phí cấu hình abilities (từ IsPriceImpacting abilities)
    /// Đây là field duy nhất staff có thể nhập cho Phase 1
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "CustomizationFee must be positive")]
    public decimal? CustomizationFee { get; set; }

    [MaxLength(2000)]
    public string? StaffDescription { get; set; }
}
