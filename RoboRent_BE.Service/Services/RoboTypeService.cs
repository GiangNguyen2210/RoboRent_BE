using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<RoboTypeLiteResponse>> GetByIdsAsync(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return new List<RoboTypeLiteResponse>();

        var normalizedIds = ids.Where(x => x > 0).Distinct().ToList();
        if (normalizedIds.Count == 0)
            return new List<RoboTypeLiteResponse>();

        var db = _repository.GetDbContext();

        var entities = await db.RoboTypes
            .Where(x => normalizedIds.Contains(x.Id) &&
                        (x.IsDeleted == null || x.IsDeleted == false))
            .ToListAsync();

        return entities
            .Select(x => _mapper.Map<RoboTypeLiteResponse>(x))
            .ToList();
    }

}