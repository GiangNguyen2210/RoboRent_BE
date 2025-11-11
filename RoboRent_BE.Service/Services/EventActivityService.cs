using AutoMapper;
using RoboRent_BE.Model.DTOs.EventActivity;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class EventActivityService: IEventActivityService
{
    private readonly IEventActivityRepository  _eventActivityRepository;
    private readonly IMapper _mapper;

    public EventActivityService(IEventActivityRepository eventActivityRepository,  IMapper mapper)
    {
        _eventActivityRepository = eventActivityRepository;
        _mapper = mapper;
    }
    
    public async Task<List<EventActivityResponse>> GetAllEventActivities()
    {
        var result = (await _eventActivityRepository.GetAllAsync()).ToList();
        var response = result.Select(ea => _mapper.Map<EventActivityResponse>(ea)).ToList();
        return response;
    }
}