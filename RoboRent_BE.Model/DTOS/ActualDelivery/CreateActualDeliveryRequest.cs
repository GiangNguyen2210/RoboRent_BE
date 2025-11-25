using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class CreateActualDeliveryRequest
{
    [Required]
    public int RentalId { get; set; }
    
    [MaxLength(2000)]
    public string? CustomerRequestNotes { get; set; }
}