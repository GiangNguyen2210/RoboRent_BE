using System.ComponentModel;

namespace RoboRent_BE.Model.DTOs.GroupSchedule;

public class GroupScheduleCreateRequest
{

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }


    public int? ActivityTypeGroupId { get; set; }
    
    public int? RentalId { get; set; }
}