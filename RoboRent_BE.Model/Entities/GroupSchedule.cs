using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class GroupSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime? EventDate { get; set; } = DateTime.UtcNow;

    public string? EventLocation { get; set; } = string.Empty;

    public string? EventCity { get; set; } =  string.Empty;

    public TimeOnly? SetupTime { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public TimeOnly? FinishTime { get; set; }

    public string? Status { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } =  false;

    public int? ActivityTypeGroupId { get; set; }

    public int? RentalId { get; set; }
    
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } =  null!;
    
    [ForeignKey("ActivityTypeGroupId")] public virtual ActivityTypeGroup ActivityTypeGroup { get; set; } =  null!;
}