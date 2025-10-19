using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Model.Entities;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ChatRoomId { get; set; }
    
    [ForeignKey(nameof(ChatRoomId))]
    public virtual ChatRoom ChatRoom { get; set; } = null!;

    [Required]
    public int SenderId { get; set; }
    
    [ForeignKey(nameof(SenderId))]
    public virtual Account Sender { get; set; } = null!;

    [Required]
    public MessageType MessageType { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    // JSON array of URLs cho demo videos/images: ["url1", "url2"]
    public string? MediaUrls { get; set; }

    // Link tới PriceQuote.Id hoặc RentalContract.Id
    public int? RelatedEntityId { get; set; }

    // Chỉ dùng cho MessageType = Demo: "Pending", "Accepted", "Rejected"
    public string? Status { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}