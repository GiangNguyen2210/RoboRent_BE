using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.ChecklistDelivery;
using RoboRent_BE.Model.DTOs.ChecklistDeliveryItem;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChecklistDeliveryController : ControllerBase
{
    private readonly IChecklistDeliveryService _checklistDeliveryService;

    public ChecklistDeliveryController(IChecklistDeliveryService checklistDeliveryService)
    {
        _checklistDeliveryService = checklistDeliveryService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChecklistAsync(ChecklistDeliveryRequest checklistDeliveryRequest)
    {
        try
        {
            var res = await _checklistDeliveryService.CreateChecklistDeliveryAsync(checklistDeliveryRequest);

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

    [HttpPut("staff/check/before/delivery")]
    public async Task<IActionResult> UpdateChecklistDeliveryBeforeDeliAsync(ChecklistUpdateDeliveryRequest  checklistUpdateDeliveryRequest)
    {
        try
        {
            var res = await _checklistDeliveryService.UpdateChecklistDeliveryAsync(checklistUpdateDeliveryRequest);
            
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