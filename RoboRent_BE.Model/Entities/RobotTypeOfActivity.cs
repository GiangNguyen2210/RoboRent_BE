using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class RobotTypeOfActivity
{
    public int? ActivityTypeId { get; set; }

    public int? RoboTypeId { get; set; }

    public int? Amount { get; set; }
    
    [ForeignKey("ActivityTypeId")] public virtual ActivityType ActivityType { get; set; } = null!;
    [ForeignKey("RoboTypeId")] public virtual RoboType RoboType { get; set; }  = null!;
}