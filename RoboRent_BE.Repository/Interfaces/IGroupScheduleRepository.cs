using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IGroupScheduleRepository : IGenericRepository<GroupSchedule>
{
    Task<GroupSchedule?> GetByRentalAsync(int rentalId);
    Task<GroupSchedule?> GetByIdAsync(int id);
    Task<IEnumerable<GroupSchedule>> GetSameDaySchedulesAsync(int groupId, DateTime date);

    // CRUD
    Task AddAsync(GroupSchedule entity);
    Task UpdateNotGenericAsync(GroupSchedule entity);

    // Required for restore logic
    void Attach(GroupSchedule entity);
    Task SaveChangesAsync();
}