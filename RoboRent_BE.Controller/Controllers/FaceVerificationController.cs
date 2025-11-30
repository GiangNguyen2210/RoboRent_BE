using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class FaceVerificationController : ControllerBase
{
    private readonly IFaceVerificationService _faceVerificationService;

    public FaceVerificationController(IFaceVerificationService faceVerificationService)
    {
        _faceVerificationService = faceVerificationService;
    }

    [HttpGet("customer/get/all/{accountId}")]
    public async Task<IActionResult> GetFaceVerificationsByAccountId(int accountId)
    {
        try
        {
            var res = await _faceVerificationService.GetAllFaceVerficationsByAccountId(accountId);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "No face verifications found for account id: " + accountId
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