using System.Linq.Expressions;
using AutoMapper;
using RoboRent_BE.Model.DTOs.RoboTypeOfActivity;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class RobotTypeOfActivityService: IRobotTypeOfActivityService
{
    private readonly IRobotTypeOfActivityRepository _repository;
    private readonly IMapper  _mapper;

    public RobotTypeOfActivityService(IRobotTypeOfActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<List<RobotTypeOfActivityResponse>> GetRoboTypeByActivityTypeIdAsync(int activityTypeId)
    {
        Expression<Func<RobotTypeOfActivity, bool>> filter = r =>
            r.ActivityTypeId == activityTypeId;
            
        var result = await _repository.GetAllAsync(filter,"RoboType,RoboType.RobotAbilities");
        
        var response = result.ToList().Select(r => _mapper.Map<RobotTypeOfActivityResponse>(r)).ToList();
        
        return response;
    }
}