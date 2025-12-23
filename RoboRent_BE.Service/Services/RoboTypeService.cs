using AutoMapper;
using RoboRent_BE.Model.DTOs.RoboType;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class RoboTypeService : IRoboTypeService
{
    private readonly IRoboTypeRepository _repository;
    private readonly IMapper _mapper;
    public RoboTypeService(IRoboTypeRepository repository,  IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<RoboTypeResponse>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(null, "RobotAbilities");
        var response = result.ToList().Select(rt => _mapper.Map<RoboTypeResponse>(rt)).ToList();
        return response;
    }
}