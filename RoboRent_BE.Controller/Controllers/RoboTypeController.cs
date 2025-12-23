using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class RoboTypeController : ControllerBase
{
    private readonly IRoboTypeService _roboTypeService;

    public RoboTypeController(IRoboTypeService roboTypeService)
    {
        _roboTypeService = roboTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var result = await _roboTypeService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}