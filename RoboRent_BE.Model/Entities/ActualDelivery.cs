using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Model.Entities;

public class ActualDelivery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int GroupScheduleId { get; set; }

    public int? StaffId { get; set; }

    [Required]
    public DeliveryType Type { get; set; } = DeliveryType.MidDay;

    public DateTime? ActualDeliveryTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(GroupScheduleId))]
    public virtual GroupSchedule GroupSchedule { get; set; } = null!;

    [ForeignKey(nameof(StaffId))]
    public virtual Account? Staff { get; set; }
}