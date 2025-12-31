using System.Linq.Expressions;
using AutoMapper;
using RoboRent_BE.Model.DTOs.ActivityType;
using RoboRent_BE.Model.DTOs.RoboType;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ActivityTypeService: IActivityTypeService
{
    private readonly IActivityTypeRepository _repository;
    private readonly IMapper _mapper;
    private readonly IRobotTypeOfActivityRepository _robotTypeOfActivityRepository;

    public ActivityTypeService(IActivityTypeRepository repository, IMapper mapper,  IRobotTypeOfActivityRepository robotTypeOfActivityRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _robotTypeOfActivityRepository = robotTypeOfActivityRepository;
    }

    public async Task<List<ActivityTypeResponse>> GetAllActivities()
    {
        var result = await _repository.GetAllAsync(
            null,
            "RobotTypeOfActivities,RobotTypeOfActivities.RoboType, RobotTypeOfActivities.RoboType.RobotAbilities"
        );

        var response = result.ToList().Select(at => _mapper.Map<ActivityTypeResponse>(at)).ToList();

        var dict = result.ToDictionary(
            a => a.Id,
            a => a.RobotTypeOfActivities
                .Where(x => x.RoboType != null)
                .Select(x => _mapper.Map<RoboTypeResponse>(x.RoboType!))
                .DistinctBy(x => x.Id) // .NET 6+
                .ToList()
        );

        foreach (var res in response)
            res.RoboTypes = dict.TryGetValue(res.Id, out var list) ? list : new();

        return response;
    }

}