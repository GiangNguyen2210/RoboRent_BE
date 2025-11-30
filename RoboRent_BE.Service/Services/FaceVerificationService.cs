using AutoMapper;
using RoboRent_BE.Model.DTOs.FaceVerifications;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class FaceVerificationService : IFaceVerificationService
{
    private readonly IFaceVerificationRepository _faceVerificationRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public FaceVerificationService(
        IFaceVerificationRepository faceVerificationRepository,
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _faceVerificationRepository = faceVerificationRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }
    
    public async Task<List<FaceVerificationsResponse>?> GetAllFaceVerficationsByAccountId(int accountId)
    {
        var account = await _accountRepository.GetAsync(a => a.Id == accountId);

        if (account == null) return null;

        var res = (await _faceVerificationRepository.GetAllAsync(fv => fv.AccountId == accountId)).ToList();
        
        return res.Select(fv => _mapper.Map<FaceVerificationsResponse>(fv)).ToList();
    }
}