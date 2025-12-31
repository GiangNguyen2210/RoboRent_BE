using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Service.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly UserManager<ModifyIdentityUser> _userManager;

    public AccountService(IAccountRepository accountRepository, UserManager<ModifyIdentityUser> userManager)
    {
        _accountRepository = accountRepository;
        _userManager = userManager;
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

    public async Task<List<Account>> GetAllStaffAccountsAsync()
    {
        // Get all users with Staff role
        var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
        var staffUserIds = staffUsers.Select(u => u.Id).ToList();

        // Get corresponding Account records
        var allAccounts = await _accountRepository.GetAllAsync();
        return allAccounts.Where(a => staffUserIds.Contains(a.UserId)).ToList();
    }

    public async Task<List<Account>> GetAllManagerAccountsAsync()
    {
        // Get all users with Manager role
        var managerUsers = await _userManager.GetUsersInRoleAsync("Manager");
        var managerUserIds = managerUsers.Select(u => u.Id).ToList();

        // Get corresponding Account records
        var allAccounts = await _accountRepository.GetAllAsync();
        return allAccounts.Where(a => managerUserIds.Contains(a.UserId)).ToList();
    }
}
