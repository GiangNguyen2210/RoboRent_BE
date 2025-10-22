using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IEventScheduleRepository : IGenericRepository<EventSchedule>
{
    Task<EventSchedule?> GetByIdAsync(int id);
    Task<List<EventSchedule>> GetByRentalIdAsync(int rentalId);
}