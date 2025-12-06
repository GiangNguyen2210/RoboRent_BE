using RoboRent_BE.Model.DTOs.ActivityTypeGroup;
using RoboRent_BE.Model.DTOs.RoboType;

namespace RoboRent_BE.Service.Interfaces;

public interface IRoboTypeService
{
    Task<IEnumerable<RoboTypeResponse>> GetAllRoboTypeAsync();
}