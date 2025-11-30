using AutoMapper;
using RoboRent_BE.Model.DTOs.FaceProfiles;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class FaceProfilesService :  IFaceProfilesService
{
    private readonly IFaceProfilesRepository _faceProfilesRepository;
    private readonly IMapper _mapper;

    public FaceProfilesService(
        IFaceProfilesRepository faceProfilesRepository,
        IMapper mapper)
    {
        _faceProfilesRepository = faceProfilesRepository;
        _mapper = mapper;
    }


    public async Task<FaceProfilesResponse?> CustomerGetFaceProfilesByAccountIdAsync(int accountId)
    {
        var faceProfile = await _faceProfilesRepository.GetAsync(fp => fp.AccountId == accountId);
        
        if (faceProfile == null) return null;
        
        return _mapper.Map<FaceProfilesResponse>(faceProfile);
    }
}