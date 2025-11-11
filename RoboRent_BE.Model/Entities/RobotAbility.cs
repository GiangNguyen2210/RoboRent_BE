using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class RobotAbility
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int? RoboTypeId { get; set; }

    public string? Ability { get; set; }

    public bool IsCustomizable { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
    
    [ForeignKey("RoboTypeId")] public virtual RoboType RoboType { get; set; } =  null!;
}