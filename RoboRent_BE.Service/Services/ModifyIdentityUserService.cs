using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    
    
}