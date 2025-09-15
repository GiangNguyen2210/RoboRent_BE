using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class TypesOfRobo
{
    public int RobotId { get; set; }
    
    public int RoboTypeId { get; set; }
    
    public bool? IsDeleted { get; set; } = false;
    
    [ForeignKey("RobotId")]
    public virtual Robot Robot { get; set; } = null!;
    
    [ForeignKey("RoboTypeId")]
    public virtual RoboType RoboType { get; set; } = null!;
}