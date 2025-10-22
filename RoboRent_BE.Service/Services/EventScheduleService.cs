using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOs.EventSchedule;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class EventScheduleService : IEventScheduleService
{
    private readonly IEventScheduleRepository _eventScheduleRepository;
    private readonly IMapper _mapper;

    public EventScheduleService(IEventScheduleRepository eventScheduleRepository,  IMapper mapper)
    {
        _eventScheduleRepository = eventScheduleRepository;
        _mapper = mapper;
    }
    public async Task<List<EventScheduleResponse>> CreateScheduleEventWithList(List<CreateEventScheduleRequest> createEventScheduleRequests)
    {
        List<EventScheduleResponse> eventScheduleResponses = new List<EventScheduleResponse>();
        foreach (var es in createEventScheduleRequests)
        {
            if (es.EventDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException($"Event date '{es.EventDate:yyyy-MM-dd}' cannot be in the past.");

            if (es.StartTime > es.EndTime)
                throw new ArgumentException($"StartTime ({es.StartTime}) must be earlier than EndTime ({es.EndTime}).");
            
            var eventSchedule = _mapper.Map<EventSchedule>(es);
            await _eventScheduleRepository.AddAsync(eventSchedule);
            eventScheduleResponses.Add(_mapper.Map<EventScheduleResponse>(eventSchedule));
        }
        return eventScheduleResponses;
    }

    public async Task<EventScheduleResponse> UpdateScheduleEvent(UpdateScheduleEventRequest updateScheduleEventRequest)
    {
        var existingEventSchedule = await _eventScheduleRepository.GetByIdAsync(updateScheduleEventRequest.Id);
        
        if (existingEventSchedule == null)
            throw new ArgumentException($"Event schedule with ID {updateScheduleEventRequest.Id} not found.");

        if (updateScheduleEventRequest.EventDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException($"Event date '{updateScheduleEventRequest.EventDate:yyyy-MM-dd}' cannot be in the past.");

        if (updateScheduleEventRequest.StartTime > updateScheduleEventRequest.EndTime)
            throw new ArgumentException($"StartTime ({updateScheduleEventRequest.StartTime}) must be earlier than EndTime ({updateScheduleEventRequest.EndTime}).");

        _mapper.Map(updateScheduleEventRequest, existingEventSchedule);
        await _eventScheduleRepository.UpdateAsync(existingEventSchedule);
        
        return _mapper.Map<EventScheduleResponse>(existingEventSchedule);
    }

    public async Task<List<EventScheduleResponse>> GetEventScheduleByRentalId(int id)
    {
        var eventSchedules = await _eventScheduleRepository.GetByRentalIdAsync(id);
        return _mapper.Map<List<EventScheduleResponse>>(eventSchedules);
    }
}