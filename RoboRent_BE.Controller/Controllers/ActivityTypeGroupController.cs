using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class ActivityTypeGroupController : ControllerBase
{
    private readonly IActivityTypeGroupService  _service;

    public ActivityTypeGroupController(IActivityTypeGroupService service)
    {
        _service = service;
    }

    [HttpGet("staff/get/all/group")]
    public async Task<IActionResult> GetAllActivityTypeGroupForStaffAsync()
    {
        try
        {
            var res = await _service.GetAllActivityTypeGroupsAsync();

            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.InnerException.Message
            });
        }
    }

    [HttpGet("staff/get/suitable/group/{rentalId}")]
    public async Task<IActionResult> GetSuitableActivityTypeGroupForRentalAsync(int rentalId)
    {
        try
        {
            var res = await _service.GetAllActivityTypeGroupsSuitableForRentalAsync(rentalId);
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.InnerException.Message
            });
        }
    }
}