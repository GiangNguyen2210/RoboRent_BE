using RoboRent_BE.Model.DTOs.GroupSchedule;

namespace RoboRent_BE.Service.Interfaces;

public interface IGroupScheduleService
{
    public Task<List<GroupScheduleGroupByDateResponse>> GetGroupScheduleByGroupId(int groupId);
    public Task<GroupScheduleResponse?> CreateGroupSchedule(GroupScheduleCreateRequest request, int staffId);
    public Task<GroupScheduleResponse?> UpdateGroupSchedule(int scheduleId,GroupScheduleUpdateRequest request);
    public Task<GroupScheduleResponse?> CancelGroupScheduleById(int scheduleId);
    public Task<GroupScheduleResponse?> CustomerGetGroupScheduleByRentalId(int rentalId);
}