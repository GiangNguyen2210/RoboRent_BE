using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Model.Entities;

/// <summary>
/// Notification entity for centralized notification storage
/// Supports the notification bell feature across all roles
/// </summary>
public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The user who will receive this notification
    /// </summary>
    [Required]
    public int RecipientId { get; set; }

    [ForeignKey(nameof(RecipientId))]
    public virtual Account Recipient { get; set; } = null!;

    /// <summary>
    /// Type of notification (from NotificationType enum)
    /// </summary>
    [Required]
    public NotificationType Type { get; set; }

    /// <summary>
    /// Display content of the notification
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional link to related rental
    /// </summary>
    public int? RentalId { get; set; }

    [ForeignKey(nameof(RentalId))]
    public virtual Rental? Rental { get; set; }

    /// <summary>
    /// Optional link to related entity (QuoteId, ContractId, DeliveryId, etc.)
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// Whether the notification has been read
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Whether the notification requires real-time push via SignalR
    /// </summary>
    public bool IsRealTime { get; set; } = true;

    /// <summary>
    /// Timestamp when the notification was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the notification was read (nullable)
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Soft delete timestamp (null = not deleted)
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
