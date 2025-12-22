using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class RoboType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string? TypeName { get; set; } =  string.Empty;
    
    public string? Description { get; set; } =  string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public virtual ICollection<RobotAbility> RobotAbilities { get; set; } = new List<RobotAbility>();
}