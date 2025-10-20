namespace RoboRent_BE.Model.DTOS.Admin;

public class UpdateUserResponse
{
    public string? Email { get; set; }
    
    public string? Role { get; set; }
    
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; }

    public string? gender { get; set; } 
    
    public bool? IdentificationIsValidated  { get; set; }
    
    public bool? isDeleted { get; set; }
    
    public string? Status { get; set; }
}