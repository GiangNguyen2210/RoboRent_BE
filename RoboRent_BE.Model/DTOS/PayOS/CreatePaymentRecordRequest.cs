using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS;

public class CreatePaymentRecordRequest
{
    [Required]
    public int RentalId { get; set; }
    
    [Required]
    public string PaymentType { get; set; } = string.Empty; // "Deposit" or "Full"
}