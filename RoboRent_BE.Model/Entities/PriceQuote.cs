using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class PriceQuote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int RentalId { get; set; }
    
    [ForeignKey(nameof(RentalId))]
    public virtual Rental Rental { get; set; } = null!;

    public Double? Delivery { get; set; }  = 0;

    public Double? Deposit { get; set; } = 0;

    public Double? Complete { get; set; } = 0;
    
    public double? Service { get; set; }
    
    public string? StaffDescription { get; set; }
    
    public string? ManagerFeedback { get; set; }
    public string? CustomerReason { get; set; } 
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public virtual Account? Manager { get; set; }

    public DateTime? SubmittedToManagerAt { get; set; }
    public DateTime? ManagerApprovedAt { get; set; }
}