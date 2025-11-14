using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class Robot
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int? RoboTypeId { get; set; }
    
    public string? RobotName { get; set; } = string.Empty;
    
    public string? ModelName { get; set; } = string.Empty;
    
    public string? Location { get; set; } = string.Empty;
    
    public string? Specification { get; set; } = string.Empty;
    
    public string? RobotStatus { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public string? Status { get; set; } = string.Empty;
    
    [ForeignKey("RoboTypeId")]  public virtual RoboType RoboType { get; set; } = null!;
}