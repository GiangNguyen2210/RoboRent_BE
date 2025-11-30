using RoboRent_BE.Model.DTOs.FaceProfiles;

namespace RoboRent_BE.Service.Interfaces;

public interface IFaceProfilesService
{
    public Task<FaceProfilesResponse?> CustomerGetFaceProfilesByAccountIdAsync(int accountId); 
}