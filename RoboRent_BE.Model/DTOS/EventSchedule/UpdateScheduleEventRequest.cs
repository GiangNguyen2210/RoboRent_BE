using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.EventSchedule;

public class UpdateScheduleEventRequest
{
    public int Id { get; set; }

    public DateTime EventDate { get; set; } =  DateTime.UtcNow;
    
    public string? Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    [Required]
    public string? EventAddress { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
}