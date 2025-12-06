using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Service.Services;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class RoboTypeController: ControllerBase
{
    private readonly IRoboTypeService  _roboTypeService;

    public RoboTypeController(IRoboTypeService roboTypeService)
    {
        _roboTypeService = roboTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoboTypes()
    {
        try
        {
            var result = await _roboTypeService.GetAllRoboTypeAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}