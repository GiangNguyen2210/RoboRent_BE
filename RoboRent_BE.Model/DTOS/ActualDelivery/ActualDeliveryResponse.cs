namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class ActualDeliveryResponse
{
    public int Id { get; set; }
    public int GroupScheduleId { get; set; }
    public int? StaffId { get; set; }
    public string? StaffName { get; set; }
    
    // Scheduled times (từ GroupSchedule)
    public DateTime? ScheduledDeliveryTime { get; set; }
    public DateTime? ScheduledPickupTime { get; set; }
    
    // Actual times (staff ghi thực tế)
    public DateTime? ActualDeliveryTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Info từ GroupSchedule
    public GroupScheduleInfo? ScheduleInfo { get; set; }
    
    // Info từ Rental
    public RentalInfo? RentalInfo { get; set; }
}

public class GroupScheduleInfo
{
    public DateTime? EventDate { get; set; }
    public string? EventLocation { get; set; }
    public string? EventCity { get; set; }
    public TimeOnly? DeliveryTime { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeOnly? FinishTime { get; set; }
}

public class RentalInfo
{
    public int RentalId { get; set; }
    public string? EventName { get; set; }
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Calendar view grouped by date
/// </summary>
public class DeliveryCalendarResponse
{
    public string Date { get; set; } = string.Empty;
    public List<ActualDeliveryResponse> Deliveries { get; set; } = new();
    public int TotalDeliveries { get; set; }
}

/// <summary>
/// Check conflict response
/// </summary>
public class ConflictCheckResponse
{
    public bool HasConflict { get; set; }
    public List<ConflictDetail> Conflicts { get; set; } = new();
}

public class ConflictDetail
{
    public int DeliveryId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
}