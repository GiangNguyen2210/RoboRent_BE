using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.EventSchedule;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Service.Services;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventScheduleController : ControllerBase
{
    private readonly IEventScheduleService  _eventScheduleService;

    public EventScheduleController(IEventScheduleService eventScheduleService)
    {
        _eventScheduleService = eventScheduleService;
    }

    [HttpPost("customer/create")]
    public async Task<IActionResult> CreateEventSchedule([FromBody] List<CreateEventScheduleRequest> createEventScheduleRequests)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }
        
        if (createEventScheduleRequests == null || !createEventScheduleRequests.Any())
            return BadRequest("The List should not be empty, Event schedule are required");

        try
        {
            var result = await _eventScheduleService.CreateScheduleEventWithList(createEventScheduleRequests);

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("customer/update")]
    public async Task<IActionResult> UpdateEventSchedule([FromBody] UpdateScheduleEventRequest updateScheduleEventRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var result = await _eventScheduleService.UpdateScheduleEvent(updateScheduleEventRequest);

            return Ok(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("customer/rental/{rentalId}")]
    public async Task<IActionResult> GetEventSchedulesByRentalId(int rentalId)
    {
        try
        {
            var result = await _eventScheduleService.GetEventScheduleByRentalId(rentalId);

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}