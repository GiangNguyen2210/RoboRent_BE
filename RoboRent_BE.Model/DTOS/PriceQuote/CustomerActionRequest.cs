using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.PriceQuote;

public class CustomerActionRequest
{
    [Required]
    public string Action { get; set; } = string.Empty; // "approve" | "reject"
    
    [MaxLength(2000)]
    public string? Reason { get; set; }
}