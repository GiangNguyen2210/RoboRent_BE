using RoboRent_BE.Model.DTOs.EventActivity;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IEventActivityService
{
    public Task<List<EventActivityResponse>> GetAllEventActivities();
}