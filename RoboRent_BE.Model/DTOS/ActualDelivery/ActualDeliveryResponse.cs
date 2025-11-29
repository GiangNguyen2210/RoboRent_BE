namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class ActualDeliveryResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public string RentalEventName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int? StaffId { get; set; }
    public string? StaffName { get; set; }
    
    public DateTime? ScheduledDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public DateTime? ScheduledPickupTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public string? CustomerRequestNotes { get; set; }
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}