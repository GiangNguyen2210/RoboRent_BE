namespace RoboRent_BE.Model.DTOs.PriceQuote;

// Response trả về thông tin báo giá
public class PriceQuoteResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public double? Delivery { get; set; }
    public double? Deposit { get; set; }
    public double? Complete { get; set; }
    public double? Service { get; set; }
    public double? DeliveryFee { get; set; }       
    public int? DeliveryDistance { get; set; }  
    public double Total { get; set; } // Calculated: Delivery + Deposit + Complete + Service
    public string? StaffDescription { get; set; }
    public string? ManagerFeedback { get; set; }
    public string? CustomerReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public int QuoteNumber { get; set; } // Thứ tự: 1, 2, hoặc 3
    public int? ManagerId { get; set; }
}