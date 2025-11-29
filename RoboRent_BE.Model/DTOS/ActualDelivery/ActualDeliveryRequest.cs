namespace RoboRent_BE.Model.DTOs.ActualDelivery;

/// <summary>
/// Tạo ActualDelivery từ GroupSchedule (trigger khi customer accept contract)
/// </summary>
public class CreateActualDeliveryRequest
{
    public int GroupScheduleId { get; set; }
}

/// <summary>
/// Manager assign staff technical cho delivery
/// </summary>
public class AssignStaffRequest
{
    public int StaffId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Staff technical update progress status
/// </summary>
public class UpdateDeliveryStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

/// <summary>
/// Staff technical update notes only
/// </summary>
public class UpdateDeliveryNotesRequest
{
    public string Notes { get; set; } = string.Empty;
}
