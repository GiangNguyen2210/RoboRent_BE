using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.PriceQuote;

// Request tạo báo giá mới
public class CreatePriceQuoteRequest
{
    [Required]
    public int RentalId { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Delivery must be positive")]
    public double? Delivery { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Deposit must be positive")]
    public double? Deposit { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Complete must be positive")]
    public double? Complete { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Service must be positive")]
    public double? Service { get; set; }
    
    [MaxLength(2000)]
    public string? StaffDescription { get; set; }
    
    [MaxLength(2000)]
    public string? ManagerFeedback { get; set; }
    
}