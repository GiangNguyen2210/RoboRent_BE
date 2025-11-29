namespace RoboRent_BE.Model.Entities;

public class ActualDelivery
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public int? StaffId { get; set; }
    
    public DateTime? ScheduledDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public DateTime? ScheduledPickupTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }
    
    public string Status { get; set; } = "Planning";
    public string? CustomerRequestNotes { get; set; }
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public Rental Rental { get; set; }
    public Account? Staff { get; set; }
}