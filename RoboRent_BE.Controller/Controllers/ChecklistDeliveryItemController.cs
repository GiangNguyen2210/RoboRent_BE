using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChecklistDeliveryItemController : ControllerBase
{
    private readonly IChecklistDeliveryItemService _service;

    public ChecklistDeliveryItemController(IChecklistDeliveryItemService service)
    {
        _service = service;
    }

    [HttpPost("{checklistDeliveryId}")]
    public async Task<IActionResult> CreateChecklistItemAsync(int checklistDeliveryId)
    {
        try
        {
            var res = await _service.CreateItemAsync(checklistDeliveryId);

            if (res == null) return NotFound(new
            {
                success = false,
                message = "Not found"
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

    [HttpGet("{checklistDeliveryId}")]
    public async Task<IActionResult> GetChecklistItemAsync(int checklistDeliveryId)
    {
        try
        {
            var res = await _service.GetAllChecklistDeliveryItemsAsync(checklistDeliveryId);

            if (res == null) return NotFound(new
            {
                success = false,
                message = "Not found"
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