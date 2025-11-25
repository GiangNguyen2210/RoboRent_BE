using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.Account;

public class UpdatePhoneNumberRequest
{
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters")]
    public string PhoneNumber { get; set; } = string.Empty;
}



