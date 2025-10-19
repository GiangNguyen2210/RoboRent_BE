using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class ChatRoom
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RentalId { get; set; }
    
    [ForeignKey(nameof(RentalId))]
    public virtual Rental Rental { get; set; } = null!;

    [Required]
    public int StaffId { get; set; }
    
    [ForeignKey(nameof(StaffId))]
    public virtual Account Staff { get; set; } = null!;

    [Required]
    public int CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Account Customer { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}