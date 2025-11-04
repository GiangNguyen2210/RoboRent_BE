using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.PriceQuote;

public class CustomerRejectRequest
{
    [MaxLength(2000)]
    public string? Reason { get; set; }
}