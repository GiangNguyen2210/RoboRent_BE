using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace RoboRent_BE.Model.Entities;

public class RentalDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public int? RentalId { get; set; }
    
    [Required] public int? RoboTypeId { get; set; }

    public DateTime CreatedAd { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAd { get; set; } = DateTime.UtcNow;

    public bool? isLocked { get; set; }
    [ForeignKey("RoboTypeId")]  public virtual RoboType RoboType { get; set; } = null!;
    
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;
}