using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class RobotTypeOfActivityController: ControllerBase
{
    private readonly IRobotTypeOfActivityService _activityTypeService;

    public RobotTypeOfActivityController(IRobotTypeOfActivityService activityTypeService)
    {
        _activityTypeService = activityTypeService;
    }

    [HttpGet("{activityTypeId}")]
    public async Task<IActionResult> GetRoboTypeByActivityTypeId(int activityTypeId)
    {
        try
        {
            var result = await _activityTypeService.GetRoboTypeByActivityTypeIdAsync(activityTypeId);
            
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}