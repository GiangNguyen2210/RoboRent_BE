using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class FaceProfilesController : ControllerBase
{
    private readonly IFaceProfilesService _faceProfilesService;

    public FaceProfilesController(IFaceProfilesService faceProfilesService)
    {
        _faceProfilesService = faceProfilesService;
    }

    [HttpGet("customer/get/faceprofiles/{accountId}")]
    public async Task<IActionResult> CustomerGetFaceProfilesByAccountIdAsync(int accountId)
    {
        try
        {
            var res = await _faceProfilesService.CustomerGetFaceProfilesByAccountIdAsync(accountId);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "Account could not be found."
            });
            
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
                message = e.Message
            });
        }
    }
}