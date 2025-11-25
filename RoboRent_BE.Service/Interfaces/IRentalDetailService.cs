using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IRentalDetailService
{
    Task<IEnumerable<RentalDetailResponse>> GetAllRentalDetailsAsync();
    Task<RentalDetailResponse?> GetRentalDetailByIdAsync(int id);
    Task<IEnumerable<RentalDetailResponse>> GetRentalDetailsByRentalIdAsync(int rentalId);
    Task<IEnumerable<RentalDetailResponse>> GetRentalDetailsByRoboTypeIdAsync(int roboTypeId);
    Task<List<RentalDetailResponse>> CreateRentalDetailAsync(List<CreateRentalDetailRequest> request);
    Task<List<RentalDetailResponse>?> UpdateRentalDetailAsync(int rentalId ,List<UpdateRentalDetailRequest> request);
    Task<bool> DeleteRentalDetailAsync(int id);
}