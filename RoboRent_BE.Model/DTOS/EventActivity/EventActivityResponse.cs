namespace RoboRent_BE.Model.DTOs.EventActivity;

public class EventActivityResponse
{
    public int Id { get; set; }
    
    public string? Name { get; set; } = string.Empty;
    
    public string? Description { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
}