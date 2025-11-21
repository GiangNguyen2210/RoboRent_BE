using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class AssignDeliveryRequest
{
    [Required]
    public DateTime ScheduledDeliveryTime { get; set; }
    
    [Required]
    public DateTime ScheduledPickupTime { get; set; }
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
}