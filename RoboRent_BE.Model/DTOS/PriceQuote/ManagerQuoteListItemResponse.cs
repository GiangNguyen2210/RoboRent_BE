namespace RoboRent_BE.Model.DTOs.PriceQuote;

/// <summary>
/// Response for Manager quote list view
/// </summary>
public class ManagerQuoteListItemResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public int QuoteNumber { get; set; }

    // Thông tin rental (cho Manager context)
    public string CustomerName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string EventDate { get; set; } = string.Empty;

    // === DEPOSIT COMPONENTS (LOCKED) ===
    public decimal RentalFee { get; set; }
    public decimal StaffFee { get; set; }
    public decimal DamageDeposit { get; set; }

    // === ADJUSTABLE FEES ===
    public decimal? DeliveryFee { get; set; }
    public int? DeliveryDistance { get; set; }
    public decimal CustomizationFee { get; set; }

    // === COMPUTED TOTALS ===
    public decimal TotalDeposit { get; set; }
    public decimal TotalPayment { get; set; }
    public decimal GrandTotal { get; set; }

    // Mô tả & feedback
    public string? StaffDescription { get; set; }
    public string? ManagerFeedback { get; set; }

    // Status & time
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}
