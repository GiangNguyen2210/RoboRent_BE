using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.RentalOrder;

public partial class CreateOrderRequest
{
    [Required]
    public string? EventName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(?:\+84|0)(3|5|7|8|9)\d{8}$",
        ErrorMessage = "Invalid Vietnamese phone number. It must start with +84 or 0 and be 10 digits total.")]
    public string? PhoneNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; } = string.Empty;
    
    [Required]
    public string? Description { get; set; } = string.Empty;
    
    [Required]
    public string? Address { get; set; } = string.Empty;
    
    [Required]
    public string? City { get; set; } = string.Empty;
    [Required]
    public TimeOnly? StartTime { get; set; }
    [Required]
    public TimeOnly? EndTime { get; set; }
    
    public DateTime? CreatedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? RequestedDate { get; set; } =  DateTime.UtcNow;
    [Required]
    [NotBeforeUtcNow(ErrorMessage = "Event date cannot be earlier than current UTC time.")]
    public DateTime? EventDate { get; set; } = DateTime.UtcNow;
    
    [DefaultValue(false)]
    public bool? IsDeleted { get; set; } =  false;
    
    [DefaultValue("Draft")]
    public string? Status { get; set; } = "Draft";
    
    [Required]
    public int? AccountId { get; set; }
    
    [Required]
    public int? ActivityTypeId  { get; set; }
}

public class NotBeforeUtcNowAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is DateTime dateTime)
        {
            if (dateTime < DateTime.UtcNow)
            {
                return new ValidationResult(
                    ErrorMessage ?? "Event date must not be earlier than the current UTC time."
                );
            }
        }

        return ValidationResult.Success;
    }
}