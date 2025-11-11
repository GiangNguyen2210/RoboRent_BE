using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class EventActivityController: ControllerBase 
{
    private readonly IEventActivityService _eventActivityService;

    public EventActivityController(IEventActivityService eventActivityService)
    {
        _eventActivityService = eventActivityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEventActivities()
    {
        try
        {
            var result = await _eventActivityService.GetAllEventActivities();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }
}