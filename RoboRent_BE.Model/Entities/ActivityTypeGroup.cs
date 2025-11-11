using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class ActivityTypeGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int ActivityTypeId { get; set; }
    
    public bool? IsDeleted { get; set; } =  false;
    
    [ForeignKey("ActivityTypeId")] public virtual ActivityType ActivityType { get; set; } = null!;
}