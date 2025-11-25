using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.EventSchedule;

public class CreateEventScheduleRequest
{
    public DateTime EventDate { get; set; } =  DateTime.UtcNow;
    
    public string? Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    [Required]
    public string? EventAddress { get; set; } = string.Empty;
    
    [DefaultValue(false)]
    public bool? IsDeleted { get; set; } =  false;
    
    public int? RentalId { get; set; }
}