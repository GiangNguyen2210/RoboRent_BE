namespace RoboRent_BE.Model.DTOs.GroupSchedule;

public class GroupScheduleResponse
{
    public DateTime? EventDate { get; set; } = DateTime.UtcNow;

    public string? EventLocation { get; set; } = string.Empty;

    public string? EventCity { get; set; } =  string.Empty;

    public TimeOnly? DeliveryTime { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public TimeOnly? FinishTime { get; set; }

    public string? Status { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } =  false;

    public int? ActivityTypeGroupId { get; set; }
    
    public int? RentalId { get; set; }
    
    public int? StaffId { get; set; }

    public string? StaffFullName { get; set; }

    public int? CustomerId { get; set; }

    public string? CustomerFullName { get; set; }
}