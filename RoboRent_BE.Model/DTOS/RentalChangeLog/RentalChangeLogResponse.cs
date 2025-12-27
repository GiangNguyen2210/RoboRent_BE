namespace RoboRent_BE.Model.DTOs.RentalChangeLog;

public class RentalChangeLogResponse
{
    public long Id { get; set; }

    public int RentalId { get; set; }

    public string FieldName { get; set; } = null!; // "StartTime", "EndTime", "EventDate", "ActivityTypeId"...

    public string? OldValue { get; set; }
    
    public string? NewValue { get; set; }

    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

    public int ChangedByAccountId { get; set; }  // ai update (customer/staff/manager)

}