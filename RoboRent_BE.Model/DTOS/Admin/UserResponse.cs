namespace RoboRent_BE.Model.DTOS.Admin;

public class UserResponse
{
    public string? Email { get; set; }
    
    public string? Role { get; set; }
    
    public string? FullName { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow;
    
    public string? gender { get; set; } = string.Empty;
    
    public bool? IdentificationIsValidated  { get; set; } = false;
    
    public bool? isDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
}