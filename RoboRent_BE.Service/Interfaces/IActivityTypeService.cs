using RoboRent_BE.Model.DTOs.ActivityType;

namespace RoboRent_BE.Service.Interfaces;

public interface IActivityTypeService
{
    public Task<List<ActivityTypeResponse>> GetAllActivitiesByEAId(int eventActivityId);
}