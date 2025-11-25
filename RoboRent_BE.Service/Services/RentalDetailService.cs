using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<RentalDetailResponse>> CreateRentalDetailAsync(List<CreateRentalDetailRequest> requests)
    {
        List<RentalDetailResponse> rentalDetailResponses = new List<RentalDetailResponse>();
        foreach (var request in requests)
        {
            var rental = await _rentalDetailRepository.GetDbContext().Rentals.AnyAsync(r => r.Id == request.RentalId);
            var roboType = await _rentalDetailRepository.GetDbContext().RoboTypes.AnyAsync(r => r.Id == request.RoboTypeId);

            if (!rental || !roboType) throw new ArgumentException("Invalid foreign key reference.");

            var rentalDetail = _mapper.Map<RentalDetail>(request);

            await _rentalDetailRepository.AddAsync(rentalDetail);
            
            rentalDetailResponses.Add(_mapper.Map<RentalDetailResponse>(rentalDetail));
        }

        return rentalDetailResponses;
    }

    public async Task<List<RentalDetailResponse>?> UpdateRentalDetailAsync(int rentalId,List<UpdateRentalDetailRequest> requests)
    {
        var rentaldetails = await _rentalDetailRepository.GetDbContext().RentalDetails.AnyAsync(r => r.RentalId == rentalId);

        if (!rentaldetails) return null;
        
        List<RentalDetailResponse> rentalDetailResponses = new List<RentalDetailResponse>();
        foreach (var request in requests)
        {
            var rental = await _rentalDetailRepository.GetDbContext().Rentals.AnyAsync(r => r.Id == request.RentalId);
            var roboType = await _rentalDetailRepository.GetDbContext().RoboTypes.AnyAsync(r => r.Id == request.RoboTypeId);

            if (!rental || !roboType) throw new ArgumentException("Invalid foreign key reference.");

            var rentalDetail = _rentalDetailRepository.GetDbContext().RentalDetails.FirstOrDefault(r => r.Id == request.Id);
            rentalDetail = _mapper.Map(request, rentalDetail);

            await _rentalDetailRepository.UpdateAsync(rentalDetail);
            
            rentalDetailResponses.Add(_mapper.Map<RentalDetailResponse>(rentalDetail));
        }
        
        return rentalDetailResponses;
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