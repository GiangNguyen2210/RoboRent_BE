using AutoMapper;
using RoboRent_BE.Model.DTOs.RoboType;
using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class RoboTypeService : IRoboTypeService
{
    private readonly IRoboTypeRepository _roboTypeRepository;
    private readonly IMapper _mapper;
    public RoboTypeService(IRoboTypeRepository roboTypeRepository, IMapper mapper)
    {
        _roboTypeRepository = roboTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoboTypeResponse>> GetAllRoboTypeAsync()
    {
        var roboTypes = await _roboTypeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoboTypeResponse>>(roboTypes);
    }
}