namespace RoboRent_BE.Model.DTOs.ActualDelivery;

public class AssignStaffBatchRequest
{
    public int StaffId { get; set; }
    public DateTime EventDate { get; set; }
    public int ActivityTypeGroupId { get; set; }
    public string? Notes { get; set; }
    public bool ForcePartialAssign { get; set; } = false;
}