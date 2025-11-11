using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Service.Services;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class ActivityTypeController: ControllerBase
{
    private readonly IActivityTypeService  _activityTypeService;

    public ActivityTypeController(IActivityTypeService activityTypeService)
    {
        _activityTypeService = activityTypeService;
    }

    [HttpGet("{eventActivityId}")]
    public async Task<IActionResult> GetAllActivityTypeByEAId(int eventActivityId)
    {
        try
        {
            var result = await _activityTypeService.GetAllActivitiesByEAId(eventActivityId);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}