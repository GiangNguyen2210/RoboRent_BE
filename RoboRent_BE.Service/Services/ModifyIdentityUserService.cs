using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.DTOS.Admin;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ModifyIdentityUserService : IModifyIdentityUserService
{
    private readonly UserManager<ModifyIdentityUser> _userManager;
    private readonly IAccountRepository  _accountRepository;
    private readonly IMapper _mapper;

    public ModifyIdentityUserService(UserManager<ModifyIdentityUser> userManager, IAccountRepository accountRepository,  IMapper mapper)
    {
        _userManager = userManager;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<IdentityResult> CreateUserAsync(CreateUserRequest  createUserRequest)
    {
        var user = new ModifyIdentityUser()
        {
            UserName = createUserRequest.Email,
            Email = createUserRequest.Email,
            EmailConfirmed = true,
        };
        
        var result = await _userManager.CreateAsync(user, createUserRequest.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, createUserRequest.Role);
        }
        return result;
    }

    public async Task<ModifyIdentityUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
    
// csharp
    public async Task<PageListResponse<StaffListItemResponse>> GetTechnicalStaffListAsync(
        int page,
        int pageSize,
        string? status = null,
        string? searchTerm = null)
    {
        var technicalUsers = await _userManager.GetUsersInRoleAsync("TechnicalStaff");
        var userIds = technicalUsers.Select(u => u.Id).ToList();
    
        var query = await _accountRepository.GetAllAsync(
            a => userIds.Contains(a.UserId) && a.isDeleted == false
        );
    
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }
    
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(a =>
                (a.FullName != null && a.FullName.ToLower().Contains(lowerSearch)) ||
                (a.PhoneNumber != null && a.PhoneNumber.Contains(searchTerm)) ||
                (a.ModifyIdentityUser.Email != null && a.ModifyIdentityUser.Email.ToLower().Contains(lowerSearch))
            );
        }
    
        var totalCount = query.Count();
    
        var items = query
            .OrderBy(a => a.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new StaffListItemResponse
            {
                AccountId = a.Id,
                UserId = a.UserId ?? string.Empty,
                Email = a.ModifyIdentityUser.Email ?? string.Empty,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Status = a.Status,
                EmailConfirmed = a.ModifyIdentityUser.EmailConfirmed
            })
            .ToList();
    
        return new PageListResponse<StaffListItemResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }

    public async Task<UpdateUserResponse?> UpdateUserAsync(UpdateUserRequest updateUserRequest)
    {
        var user = await _userManager.FindByEmailAsync(updateUserRequest.Email);

        if (user == null) return null;
        
        user.Email = updateUserRequest.Email;
        user.UserName = updateUserRequest.Email;

        var rolesOfUser = await _userManager.GetRolesAsync(user);
        
        await _userManager.RemoveFromRolesAsync(user, rolesOfUser);
        
        await _userManager.AddToRoleAsync(user, updateUserRequest.Role);

        if (!string.IsNullOrEmpty(updateUserRequest.Password))
        {
            // Remove the old password hash
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                throw new Exception(string.Join(", ", removeResult.Errors.Select(e => e.Description)));
            }

            // Add the new password (this automatically re-hashes it)
            var addResult = await _userManager.AddPasswordAsync(user, updateUserRequest.Password);
            if (!addResult.Succeeded)
            {
                throw new Exception(string.Join(", ", addResult.Errors.Select(e => e.Description)));
            }
        }


        var account = await _accountRepository.GetByUserIdAsync(user.Id);

        if (account == null)  return null;
        
        account.FullName = updateUserRequest.FullName;
        account.PhoneNumber = updateUserRequest.PhoneNumber;
        account.DateOfBirth = updateUserRequest.DateOfBirth;
        account.gender = updateUserRequest.gender;
        account.IdentificationIsValidated = updateUserRequest.IdentificationIsValidated;
        account.isDeleted = updateUserRequest.isDeleted;
        account.Status =  updateUserRequest.Status;
        
        await _accountRepository.UpdateAsync(account);

        var response = _mapper.Map<UpdateUserResponse>(user);
        response.Role = (await _userManager.GetRolesAsync(user))[0];
        _mapper.Map(account, response);
        
        return response;
    }
    
    public async Task<PageListResponse<StaffListItemResponse>> GetStaffListAsync(
        int page, 
        int pageSize, 
        string? status = null, 
        string? searchTerm = null)
    {
        // Get all users in Staff role
        var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
        var userIds = staffUsers.Select(u => u.Id).ToList();

        // Get accounts
        var query = await _accountRepository.GetAllAsync(
            a => userIds.Contains(a.UserId) && a.isDeleted == false
        );

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        // Search by FullName, PhoneNumber, Email
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(a => 
                (a.FullName != null && a.FullName.ToLower().Contains(lowerSearch)) ||
                (a.PhoneNumber != null && a.PhoneNumber.Contains(searchTerm)) ||
                (a.ModifyIdentityUser.Email != null && a.ModifyIdentityUser.Email.ToLower().Contains(lowerSearch))
            );
        }

        var totalCount = query.Count();

        // Pagination
        var items = query
            .OrderBy(a => a.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new StaffListItemResponse
            {
                AccountId = a.Id,
                UserId = a.UserId ?? string.Empty,
                Email = a.ModifyIdentityUser.Email ?? string.Empty,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Status = a.Status,
                EmailConfirmed = a.ModifyIdentityUser.EmailConfirmed
            })
            .ToList();

        return new PageListResponse<StaffListItemResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }
    
    public async Task<PageListResponse<StaffListItemResponse>> GetManagerListAsync(
        int page, 
        int pageSize, 
        string? status = null, 
        string? searchTerm = null)
    {
        // Get all users in Manager role
        var managerUsers = await _userManager.GetUsersInRoleAsync("Manager");
        var userIds = managerUsers.Select(u => u.Id).ToList();

        // Get accounts
        var query = await _accountRepository.GetAllAsync(
            a => userIds.Contains(a.UserId) && a.isDeleted == false
        );

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        // Search by FullName, PhoneNumber, Email
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(a => 
                (a.FullName != null && a.FullName.ToLower().Contains(lowerSearch)) ||
                (a.PhoneNumber != null && a.PhoneNumber.Contains(searchTerm)) ||
                (a.ModifyIdentityUser.Email != null && a.ModifyIdentityUser.Email.ToLower().Contains(lowerSearch))
            );
        }

        var totalCount = query.Count();

        // Pagination
        var items = query
            .OrderBy(a => a.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new StaffListItemResponse
            {
                AccountId = a.Id,
                UserId = a.UserId ?? string.Empty,
                Email = a.ModifyIdentityUser.Email ?? string.Empty,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Status = a.Status,
                EmailConfirmed = a.ModifyIdentityUser.EmailConfirmed
            })
            .ToList();

        return new PageListResponse<StaffListItemResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PageListResponse<AccountListResponse>> GetAllAccountsAsync(
        int page,
        int pageSize,
        string? status = null,
        string? searchTerm = null)
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        var adminUserIds = adminUsers.Select(u => u.Id).ToList();
        var nonAdminUserIds = allUsers.Where(u => !adminUserIds.Contains(u.Id)).Select(u => u.Id).ToList();

        var query = await _accountRepository.GetAllAsync(
            a => nonAdminUserIds.Contains(a.UserId) && a.isDeleted == false
        );

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(a =>
                (a.FullName != null && a.FullName.ToLower().Contains(lowerSearch)) ||
                (a.ModifyIdentityUser.Email != null && a.ModifyIdentityUser.Email.ToLower().Contains(lowerSearch))
            );
        }

        var totalCount = query.Count();

        var items = query
            .OrderBy(a => a.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AccountListResponse
            {
                AccountId = a.Id,
                UserId = a.UserId ?? string.Empty,
                Email = a.ModifyIdentityUser.Email ?? string.Empty,
                FullName = a.FullName,
                PhoneNumber = string.Empty,
                Status = a.Status,
                EmailConfirmed = a.ModifyIdentityUser.EmailConfirmed,
            })
            .ToList();

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.UserId))
            {
                var user = await _userManager.FindByIdAsync(item.UserId);
                item.Role = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "Customer";
            }
        }

        return new PageListResponse<AccountListResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }

    public async Task<bool> UpdateUserStatusAsync(int accountId, string status)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
        {
            return false;
        }

        account.Status = status;
        await _accountRepository.UpdateAsync(account);
        return true;
    }
}