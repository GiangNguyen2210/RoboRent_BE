using AutoMapper;
using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class RentalDetailService : IRentalDetailService
{
    private readonly IRentalDetailRepository _rentalDetailRepository;
    private readonly IMapper _mapper;

    public RentalDetailService(IRentalDetailRepository rentalDetailRepository, IMapper mapper)
    {
        _rentalDetailRepository = rentalDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RentalDetailResponse>> GetAllRentalDetailsAsync()
    {
        var rentalDetails = await _rentalDetailRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<RentalDetailResponse>>(rentalDetails);
    }

    public async Task<RentalDetailResponse?> GetRentalDetailByIdAsync(int id)
    {
        var rentalDetail = await _rentalDetailRepository.GetAsync(rd => rd.Id == id, "RoboType,Rental");
        if (rentalDetail == null || rentalDetail.IsDeleted == true)
            return null;

        return _mapper.Map<RentalDetailResponse>(rentalDetail);
    }

    public async Task<IEnumerable<RentalDetailResponse>> GetRentalDetailsByRentalIdAsync(int rentalId)
    {
        var rentalDetails = await _rentalDetailRepository.GetRentalDetailsByRentalIdAsync(rentalId);
        return _mapper.Map<IEnumerable<RentalDetailResponse>>(rentalDetails);
    }

    public async Task<IEnumerable<RentalDetailResponse>> GetRentalDetailsByRoboTypeIdAsync(int roboTypeId)
    {
        var rentalDetails = await _rentalDetailRepository.GetRentalDetailsByRoboTypeIdAsync(roboTypeId);
        return _mapper.Map<IEnumerable<RentalDetailResponse>>(rentalDetails);
    }

    public async Task<RentalDetailResponse> CreateRentalDetailAsync(CreateRentalDetailRequest request)
    {
        var rentalDetail = _mapper.Map<RentalDetail>(request);
        var createdRentalDetail = await _rentalDetailRepository.AddAsync(rentalDetail);
        
        return _mapper.Map<RentalDetailResponse>(createdRentalDetail);
    }

    public async Task<RentalDetailResponse?> UpdateRentalDetailAsync(UpdateRentalDetailRequest request)
    {
        var existingRentalDetail = await _rentalDetailRepository.GetAsync(rd => rd.Id == request.Id, "RoboType,Rental");
        if (existingRentalDetail == null || existingRentalDetail.IsDeleted == true)
            return null;

        _mapper.Map(request, existingRentalDetail);
        var updatedRentalDetail = await _rentalDetailRepository.UpdateAsync(existingRentalDetail);
        
        return _mapper.Map<RentalDetailResponse>(updatedRentalDetail);
    }

    public async Task<bool> DeleteRentalDetailAsync(int id)
    {
        var rentalDetail = await _rentalDetailRepository.GetAsync(rd => rd.Id == id);
        if (rentalDetail == null || rentalDetail.IsDeleted == true)
            return false;

        rentalDetail.IsDeleted = true;
        await _rentalDetailRepository.UpdateAsync(rentalDetail);
        return true;
    }
}