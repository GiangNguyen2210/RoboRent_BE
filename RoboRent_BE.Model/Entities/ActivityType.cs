using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace RoboRent_BE.Model.Entities;

public partial class ActivityType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int EventActivityId { get; set; }
    
    public string? Name { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
    
    [ForeignKey("EventActivityId")] public virtual EventActivity EventActivity { get; set; } = null!;
    
    public virtual ICollection<ActivityTypeGroup> ActivityTypeGroups { get; set; } = new List<ActivityTypeGroup>();

}