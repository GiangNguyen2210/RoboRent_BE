using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    
    public int? RobotAbilityId { get; set; }

    public string? Script { get; set; } =  string.Empty;

    public string? Branding { get; set; } = string.Empty;

    public string? Scenario { get; set; } =  string.Empty;

    [ForeignKey("RobotAbilityId")] public virtual RobotAbility RobotAbility { get; set; } = null!;
    
    [ForeignKey("RoboTypeId")]  public virtual RoboType RoboType { get; set; } = null!;
    
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;
}