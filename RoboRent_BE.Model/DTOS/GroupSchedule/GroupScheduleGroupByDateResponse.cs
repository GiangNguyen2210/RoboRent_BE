namespace RoboRent_BE.Model.DTOs.GroupSchedule;

public class GroupScheduleGroupByDateResponse
{
    public DateTime? EventDate { get; set; }
    public List<GroupScheduleResponse> Items { get; set; } = new();
}
