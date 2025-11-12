using RoboRent_BE.Model.DTOs.ActivityTypeGroup;

namespace RoboRent_BE.Service.Interfaces;

public interface IActivityTypeGroupService
{
    public Task<List<ActivityTypeGroupResponse>> GetAllActivityTypeGroupsAsync();

    public Task<List<ActivityTypeGroupResponse>> GetAllActivityTypeGroupsSuitableForRentalAsync(int rentalId);
}