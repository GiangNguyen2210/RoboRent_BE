using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class RentalChangeLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public int RentalId { get; set; }

    [MaxLength(100)]
    public string FieldName { get; set; } = null!; // "StartTime", "EndTime", "EventDate", "ActivityTypeId"...

    public string? OldValue { get; set; }
    
    public string? NewValue { get; set; }

    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

    public int ChangedByAccountId { get; set; }  // ai update (customer/staff/manager)

    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;
}