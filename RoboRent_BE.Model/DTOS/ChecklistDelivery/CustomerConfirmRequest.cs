namespace RoboRent_BE.Model.DTOs.ChecklistDelivery;

public class CustomerConfirmRequest
{
    public DateTime? CustomerAcceptedAt { get; set; }
    
    public int? CustomerAcceptedById { get; set; }
    
    public string? CustomerNote { get; set; }
}