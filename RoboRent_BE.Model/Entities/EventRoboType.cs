using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class EventRoboType
{
    public int EventId { get; set; }
    
    public int RoboTypeId { get; set; }
    
    public bool? IsDeleted { get; set; } =  false;
    
    [ForeignKey("EventId")] public virtual Event Event { get; set; } = null!;
    
    [ForeignKey("RoboTypeId")] public virtual RoboType RoboType { get; set; } = null!;
}