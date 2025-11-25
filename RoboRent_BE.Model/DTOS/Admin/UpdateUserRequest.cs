using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.Admin;

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }
    
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#%^&+=!]).{8,}$",
        ErrorMessage = "Password must contain at least one upper case letter, one lower case letter, one number and one special character like @, #, $, %, !."
    )]
    public string? Password { get; set; }
    
    [Required]
    [EnumDataType(typeof(Roles), ErrorMessage = "you should select a valid Role")]
    public string? Role { get; set; }
    
    public string? FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^(?:\+84|0)(3|5|7|8|9)\d{8}$",
        ErrorMessage = "Invalid Vietnamese phone number. It must start with +84 or 0 and be 10 digits total.")]
    public string? PhoneNumber { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow;
    
    [EnumDataType(typeof(Genders), ErrorMessage = "you should select a valid Gender")]
    public string? gender { get; set; } = string.Empty;
    
    public bool? IdentificationIsValidated  { get; set; } = false;
    
    public bool? isDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
}

enum Genders
{
    male,
    female,
    other
}