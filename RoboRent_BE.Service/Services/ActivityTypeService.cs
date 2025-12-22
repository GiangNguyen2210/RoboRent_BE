using System.Linq.Expressions;
using AutoMapper;
using RoboRent_BE.Model.DTOs.ActivityType;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ActivityTypeService: IActivityTypeService
{
    private readonly IActivityTypeRepository _repository;
    private readonly IMapper _mapper;

    public ActivityTypeService(IActivityTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ActivityTypeResponse>> GetAllActivitiesByEAId(int eventActivityId)
    {
        // Expression<Func<ActivityType, bool>> filter = at => at.EventActivityId == eventActivityId;
        var result = await _repository.GetAllAsync();
        var response = result.ToList().Select(at => _mapper.Map<ActivityTypeResponse>(at)).ToList();
        return response;
    }
}