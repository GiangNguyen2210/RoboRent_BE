using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class RentalDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? Amount { get; set; } = 0;

    public string? Script { get; set; } = string.Empty;
    
    public string? Branding { get; set; } = string.Empty;

    public string? Dance { get; set; } = string.Empty;

    public string? Music { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public int? RentalId { get; set; }
    
    [Required] public int? RoboTypeId { get; set; }
    
    [ForeignKey("RoboTypeId")]  public virtual RoboType RoboType { get; set; } = null!;
    
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;
}