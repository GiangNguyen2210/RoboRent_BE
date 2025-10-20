using System.ComponentModel.DataAnnotations;
using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Model.DTOs.Chat;

public class SendMessageRequest
{
    [Required]
    public int RentalId { get; set; }
    
    [Required]
    public MessageType MessageType { get; set; } // Text, Demo, etc.
    
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
    
    // Optional: Chỉ dùng khi MessageType = Demo
    public List<string>? VideoUrls { get; set; }
    
    // Optional: Chỉ dùng khi MessageType = PriceQuoteNotification/ContractNotification
    public int? RelatedEntityId { get; set; }
}