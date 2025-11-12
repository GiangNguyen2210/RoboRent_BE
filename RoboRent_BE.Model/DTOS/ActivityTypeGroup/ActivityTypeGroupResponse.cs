namespace RoboRent_BE.Model.DTOs.ActivityTypeGroup;

public class ActivityTypeGroupResponse
{
    public int Id { get; set; }
    
    public int ActivityTypeId { get; set; }
    
    public bool? IsDeleted { get; set; } =  false;
    
    public int EventActivityId { get; set; }
    
    public string? ActivityTypeName { get; set; } = string.Empty;
    
    public string? EventActivityName { get; set; }

}