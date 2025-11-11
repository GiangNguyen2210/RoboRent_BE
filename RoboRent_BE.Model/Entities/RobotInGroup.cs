using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class RobotInGroup
{
    public int? ActivityTypeGroupId { get; set; }

    public int? RobotId { get; set; }
    
    public bool? IsDeleted { get; set; } =  false;

    [ForeignKey("ActivityTypeGroupId")] public virtual ActivityTypeGroup ActivityTypeGroup { get; set; } = null!;
    [ForeignKey("RobotId")] public virtual Robot Robot { get; set; }  = null!;
}