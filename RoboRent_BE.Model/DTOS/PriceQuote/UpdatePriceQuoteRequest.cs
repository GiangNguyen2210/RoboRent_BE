using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.PriceQuote;

/// <summary>
/// Request cập nhật báo giá (Phase 2)
/// Chỉ cho phép sửa DeliveryFee và CustomizationFee
/// RentalFee, StaffFee, DamageDeposit là LOCKED
/// </summary>
public class UpdatePriceQuoteRequest
{
    /// <summary>
    /// Phí giao hàng (có thể sửa nếu địa chỉ thay đổi)
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "DeliveryFee must be positive")]
    public decimal? DeliveryFee { get; set; }

    /// <summary>
    /// Phí cấu hình abilities (từ IsPriceImpacting abilities)
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "CustomizationFee must be positive")]
    public decimal? CustomizationFee { get; set; }

    [MaxLength(2000)]
    public string? StaffDescription { get; set; }
}
