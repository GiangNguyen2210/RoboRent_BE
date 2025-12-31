using RoboRent_BE.Model.DTOs.ActivityTypeGroup;

namespace RoboRent_BE.Service.Interfaces;

public interface IActivityTypeGroupService
{
    public Task<List<ActivityTypeGroupResponse>> GetAllActivityTypeGroupsAsync();

    public Task<List<ActivityTypeGroupResponse>> GetAllActivityTypeGroupsSuitableForRentalAsync(int rentalId);
    public Task<ActivityTypeGroupResponse?> GetActivityTypeGroupByIdAsync(int activityTypeGroupId);
    
    public Task<List<ActivityTypeGroupResponse>?> GetGroupByActivityTypeId(int activityTypeId);
}