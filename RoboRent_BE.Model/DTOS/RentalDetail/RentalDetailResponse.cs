namespace RoboRent_BE.Model.DTOS.RentalDetail;

public class RentalDetailResponse
{
    public int Id { get; set; }

    public int? Amount { get; set; } = 0;

    public string? Script { get; set; } = string.Empty;
    
    public string? Branding { get; set; } = string.Empty;

    public string? Dance { get; set; } = string.Empty;

    public string? Music { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    public int? RentalId { get; set; }
    
    public int? RoboTypeId { get; set; }
    
    // Navigation properties
    public string? RoboTypeName { get; set; }
    public string? RentalEventName { get; set; }
}


