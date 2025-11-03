using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.PriceQuote;

public class ManagerRejectRequest
{
    [Required]
    [MaxLength(2000)]
    public string Feedback { get; set; } = string.Empty;
}