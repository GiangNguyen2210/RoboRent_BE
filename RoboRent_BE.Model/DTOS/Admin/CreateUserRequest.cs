using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.Admin;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#%^&+=!]).{8,}$",
        ErrorMessage = "Password must contain at least one upper case letter, one lower case letter, one number and one special character like @, #, $, %, !."
        )]
    public string? Password { get; set; }
    
    [Required]
    [EnumDataType(typeof(Roles), ErrorMessage = "you should select a valid Role")]
    public string? Role { get; set; }
}

enum Roles
{
    Admin,
    Customer,
    Staff
}