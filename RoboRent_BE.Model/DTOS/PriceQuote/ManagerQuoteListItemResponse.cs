namespace RoboRent_BE.Model.DTOs.PriceQuote;

public class ManagerQuoteListItemResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public int QuoteNumber { get; set; }
    
    // Thông tin rental (cho Manager context)
    public string CustomerName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string EventDate { get; set; } = string.Empty;
    
    // Thông tin giá
    public double? DeliveryFee { get; set; }
    public int? DeliveryDistance { get; set; }
    public double? Deposit { get; set; }
    public double? Complete { get; set; }
    public double? Service { get; set; }
    public double Total { get; set; }
    
    // Mô tả & feedback
    public string? StaffDescription { get; set; }
    public string? ManagerFeedback { get; set; }
    
    // Status & time
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}