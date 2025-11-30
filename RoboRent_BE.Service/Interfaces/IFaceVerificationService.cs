using RoboRent_BE.Model.DTOs.FaceVerifications;

namespace RoboRent_BE.Service.Interfaces;

public interface IFaceVerificationService
{
    public Task<List<FaceVerificationsResponse>?> GetAllFaceVerficationsByAccountId(int accountId);
}