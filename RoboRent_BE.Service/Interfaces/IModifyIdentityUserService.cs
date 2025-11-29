using Microsoft.AspNetCore.Identity;
using RoboRent_BE.Model.DTOS.Admin;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IModifyIdentityUserService
{
    // Add custom methods here
    public Task<IdentityResult> CreateUserAsync(CreateUserRequest createUserRequest);
    
    public Task<ModifyIdentityUser?> GetUserByEmailAsync(string email);
    
    public Task<UpdateUserResponse?> UpdateUserAsync(UpdateUserRequest updateUserRequest);
}