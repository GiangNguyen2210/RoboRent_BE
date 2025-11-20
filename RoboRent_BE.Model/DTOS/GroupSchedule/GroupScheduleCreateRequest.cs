namespace RoboRent_BE.Model.DTOs.GroupSchedule;

public class GroupScheduleCreateRequest
{
    public TimeOnly? DeliveryTime { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public TimeOnly? FinishTime { get; set; }

    public int? ActivityTypeGroupId { get; set; }
    
    public int? RentalId { get; set; }
}