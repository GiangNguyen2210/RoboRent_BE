namespace RoboRent_BE.Model.DTOs.PriceQuote;

/// <summary>
/// Response trả về thông tin báo giá
/// </summary>
public class PriceQuoteResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }

    // === DEPOSIT COMPONENTS (LOCKED) ===
    public decimal RentalFee { get; set; }
    public decimal StaffFee { get; set; }
    public decimal DamageDeposit { get; set; }

    // === ADJUSTABLE FEES ===
    public decimal? DeliveryFee { get; set; }
    public int? DeliveryDistance { get; set; }
    public decimal CustomizationFee { get; set; }

    // === COMPUTED TOTALS ===
    public decimal TotalDeposit { get; set; }  // 30% × (RentalFee + StaffFee) + DamageDeposit
    public decimal TotalPayment { get; set; }  // 70% × (RentalFee + StaffFee) + DeliveryFee + CustomizationFee
    public decimal GrandTotal { get; set; }    // TotalDeposit + TotalPayment

    // === METADATA ===
    public string? StaffDescription { get; set; }
    public string? ManagerFeedback { get; set; }
    public string? CustomerReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public int QuoteNumber { get; set; }
    public int? ManagerId { get; set; }
}
