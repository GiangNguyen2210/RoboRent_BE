namespace RoboRent_BE.Model.DTOs.ActivityType;

public class ActivityTypeResponse
{
    public int Id { get; set; }
    
    public int EventActivityId { get; set; }
    
    public string? Name { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
}