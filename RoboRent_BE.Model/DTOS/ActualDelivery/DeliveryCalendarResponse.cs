namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class DeliveryCalendarResponse
{
    public string Date { get; set; } = string.Empty; // Format: "Nov 21, 2025"
    public List<ActualDeliveryResponse> Deliveries { get; set; } = new();
    public int TotalDeliveries { get; set; }
}