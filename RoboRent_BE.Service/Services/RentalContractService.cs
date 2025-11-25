using AutoMapper;
using RoboRent_BE.Model.DTOS.RentalContract;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class RentalContractService : IRentalContractService
{
    private readonly IRentalContractRepository _rentalContractRepository;
    private readonly IMapper _mapper;

    public RentalContractService(IRentalContractRepository rentalContractRepository, IMapper mapper)
    {
        _rentalContractRepository = rentalContractRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RentalContractResponse>> GetAllRentalContractsAsync()
    {
        var rentalContracts = await _rentalContractRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<RentalContractResponse>>(rentalContracts);
    }

    public async Task<RentalContractResponse?> GetRentalContractByIdAsync(int id)
    {
        var rentalContract = await _rentalContractRepository.GetAsync(rc => rc.Id == id, "Rental");
        if (rentalContract == null || rentalContract.IsDeleted == true)
            return null;

        return _mapper.Map<RentalContractResponse>(rentalContract);
    }

    public async Task<IEnumerable<RentalContractResponse>> GetRentalContractsByRentalIdAsync(int rentalId)
    {
        var rentalContracts = await _rentalContractRepository.GetRentalContractsByRentalIdAsync(rentalId);
        return _mapper.Map<IEnumerable<RentalContractResponse>>(rentalContracts);
    }

    public async Task<RentalContractResponse> CreateRentalContractAsync(CreateRentalContractRequest request)
    {
        var rentalContract = _mapper.Map<RentalContract>(request);
        var createdRentalContract = await _rentalContractRepository.AddAsync(rentalContract);
        
        return _mapper.Map<RentalContractResponse>(createdRentalContract);
    }

    public async Task<RentalContractResponse?> UpdateRentalContractAsync(UpdateRentalContractRequest request)
    {
        var existingRentalContract = await _rentalContractRepository.GetAsync(rc => rc.Id == request.Id, "Rental");
        if (existingRentalContract == null || existingRentalContract.IsDeleted == true)
            return null;

        _mapper.Map(request, existingRentalContract);
        var updatedRentalContract = await _rentalContractRepository.UpdateAsync(existingRentalContract);
        
        return _mapper.Map<RentalContractResponse>(updatedRentalContract);
    }

    public async Task<bool> DeleteRentalContractAsync(int id)
    {
        var rentalContract = await _rentalContractRepository.GetAsync(rc => rc.Id == id);
        if (rentalContract == null || rentalContract.IsDeleted == true)
            return false;

        rentalContract.IsDeleted = true;
        await _rentalContractRepository.UpdateAsync(rentalContract);
        return true;
    }
}
