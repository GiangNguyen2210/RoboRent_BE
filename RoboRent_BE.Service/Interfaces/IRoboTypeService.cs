using RoboRent_BE.Model.DTOs.RoboType;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IRoboTypeService
{
    // Add custom methods here
    public Task<List<RoboTypeResponse>> GetAllAsync();
}