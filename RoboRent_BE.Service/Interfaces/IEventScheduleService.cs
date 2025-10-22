using RoboRent_BE.Model.DTOs.EventSchedule;

namespace RoboRent_BE.Service.Interfaces;

public interface IEventScheduleService
{
    public Task<List<EventScheduleResponse>> CreateScheduleEventWithList(List<CreateEventScheduleRequest> createEventScheduleRequests);
    public Task<EventScheduleResponse> UpdateScheduleEvent(UpdateScheduleEventRequest updateScheduleEventRequest);
    public Task<List<EventScheduleResponse>> GetEventScheduleByRentalId(int id);
}