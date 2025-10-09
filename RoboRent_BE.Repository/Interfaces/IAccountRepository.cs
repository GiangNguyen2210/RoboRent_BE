using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByIdAsync(int accountId);
    // Add custom methods here
}