using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

[Table("PaymentRecords")]
public class PaymentRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? RentalId { get; set; }
    
    [ForeignKey(nameof(RentalId))]
    public virtual Rental? Rental { get; set; }

    public int? PriceQuoteId { get; set; }
    
    [ForeignKey(nameof(PriceQuoteId))]
    public virtual PriceQuote? PriceQuote { get; set; }

    [Required]
    public string PaymentType { get; set; } = string.Empty; // "Deposit" or "Full"

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public long OrderCode { get; set; } // PayOS orderCode

    public string? PaymentLinkId { get; set; } // PayOS paymentLinkId

    [Required]
    public string Status { get; set; } = "Pending"; // "Pending", "Paid", "Failed", "Cancelled"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PaidAt { get; set; }
}