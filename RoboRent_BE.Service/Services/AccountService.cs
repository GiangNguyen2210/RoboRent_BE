using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Service.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account> CreatePendingAccountAsync(string userId, string? fullName)
    {
        // check if account exists
        var existing = await _accountRepository.GetAsync(a => a.UserId == userId);
        if (existing != null)
        {
            return existing;
        }

        var account = new Account
        {
            UserId = userId,
            FullName = fullName ?? string.Empty,
            Status = "PendingVerification",
            isDeleted = false
        };

        return await _accountRepository.AddAsync(account);
    }

    public async Task<bool> ActivateAccountAsync(string userId)
    {
        var account = await _accountRepository.GetAsync(a => a.UserId == userId);
        if (account == null)
        {
            return false;
        }

        account.Status = "Active";
        await _accountRepository.UpdateAsync(account);
        return true;
    }

    public async Task<Account?> GetAccountByUserIdAsync(string userId)
    {
        return await _accountRepository.GetAsync(a => a.UserId == userId);
    }

    public async Task<bool> UpdatePhoneNumberAsync(string userId, string phoneNumber)
    {
        var account = await _accountRepository.GetAsync(a => a.UserId == userId);
        if (account == null)
        {
            return false;
        }

        account.PhoneNumber = phoneNumber;
        await _accountRepository.UpdateAsync(account);
        return true;
    }
}