using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class UpdateDeliveryStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty; // Planning, Assigned, Delivering, Delivered, Collecting, Collected, Completed
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
}