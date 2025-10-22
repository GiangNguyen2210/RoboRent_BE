namespace RoboRent_BE.Model.DTOs.EventSchedule;

public class EventScheduleResponse
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; } =  DateTime.UtcNow;
    
    public string? Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public string? EventAddress { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public int? RentalId { get; set; }
}