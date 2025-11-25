using RoboRent_BE.Model.DTOs.RoboTypeOfActivity;

namespace RoboRent_BE.Service.Interfaces;

public interface IRobotTypeOfActivityService
{
    public Task<List<RobotTypeOfActivityResponse>> GetRoboTypeByActivityTypeIdAsync(int activityTypeId);
}