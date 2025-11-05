using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IRentalDetailService
{
    Task<IEnumerable<RentalDetailResponse>> GetAllRentalDetailsAsync();
    Task<RentalDetailResponse?> GetRentalDetailByIdAsync(int id);
    Task<IEnumerable<RentalDetailResponse>> GetRentalDetailsByRentalIdAsync(int rentalId);
    Task<IEnumerable<RentalDetailResponse>> GetRentalDetailsByRoboTypeIdAsync(int roboTypeId);
    Task<RentalDetailResponse> CreateRentalDetailAsync(CreateRentalDetailRequest request);
    Task<RentalDetailResponse?> UpdateRentalDetailAsync(UpdateRentalDetailRequest request);
    Task<bool> DeleteRentalDetailAsync(int id);
}