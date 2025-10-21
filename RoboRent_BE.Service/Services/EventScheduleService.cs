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
        var es = await _eventScheduleRepository.GetDbContext().EventSchedules.FirstOrDefaultAsync(es => es.Id == updateScheduleEventRequest.Id);
        throw new NotImplementedException();
    }

    public Task<List<EventScheduleResponse>> GetEventScheduleByRentalId(int id)
    {
        throw new NotImplementedException();
    }
}