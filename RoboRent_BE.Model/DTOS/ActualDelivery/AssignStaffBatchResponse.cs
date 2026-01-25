namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class AssignStaffBatchResponse
{
    public bool Success { get; set; }
    public int AssignedCount { get; set; }
    public bool HasConflict { get; set; }
    public List<int>? ConflictingScheduleIds { get; set; }
    public List<int>? AssignedScheduleIds { get; set; }
    public string? ConflictMessage { get; set; }
}
