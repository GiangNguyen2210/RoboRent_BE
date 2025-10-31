using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IAccountService
{
    Task<Account> CreatePendingAccountAsync(string userId, string? fullName);
    Task<bool> ActivateAccountAsync(string userId);
    Task<Account?> GetAccountByUserIdAsync(string userId);
    Task<bool> UpdatePhoneNumberAsync(string userId, string phoneNumber);
}