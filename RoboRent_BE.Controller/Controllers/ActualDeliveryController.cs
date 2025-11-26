using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.ActualDelivery;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActualDeliveryController : ControllerBase
{
    private readonly IActualDeliveryService _deliveryService;

    public ActualDeliveryController(IActualDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("AccountId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return int.Parse(userIdClaim);
    }

    /// <summary>
    /// [AUTO/SYSTEM] Tạo ActualDelivery khi customer accept contract
    /// Trigger từ contract acceptance flow
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDelivery([FromBody] CreateActualDeliveryRequest request)
    {
        try
        {
            var delivery = await _deliveryService.CreateActualDeliveryAsync(request);
            return Ok(new
            {
                success = true,
                message = "ActualDelivery created successfully",
                data = delivery
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to create delivery",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [MANAGER] Assign staff technical cho delivery
    /// </summary>
    [HttpPut("{id}/assign-staff")]
    public async Task<IActionResult> AssignStaff(int id, [FromBody] AssignStaffRequest request)
    {
        try
        {
            var delivery = await _deliveryService.AssignStaffAsync(id, request);
            return Ok(new
            {
                success = true,
                message = "Staff assigned successfully",
                data = delivery
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to assign staff",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [MANAGER] Check conflict trước khi assign staff
    /// </summary>
    [HttpGet("check-conflict")]
    public async Task<IActionResult> CheckConflict(
        [FromQuery] int staffId, 
        [FromQuery] int groupScheduleId)
    {
        try
        {
            var result = await _deliveryService.CheckStaffConflictAsync(staffId, groupScheduleId);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to check conflict",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [STAFF] Update delivery status (progress tracking)
    /// Pending → Assigned → Delivering → Delivered → Collecting → Collected → Completed
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateDeliveryStatusRequest request)
    {
        try
        {
            int staffId = GetCurrentUserId();
            var delivery = await _deliveryService.UpdateStatusAsync(id, request, staffId);
            return Ok(new
            {
                success = true,
                message = "Status updated successfully",
                data = delivery
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to update status",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [STAFF] Update notes only
    /// </summary>
    [HttpPut("{id}/notes")]
    public async Task<IActionResult> UpdateNotes(int id, [FromBody] UpdateDeliveryNotesRequest request)
    {
        try
        {
            int staffId = GetCurrentUserId();
            var delivery = await _deliveryService.UpdateNotesAsync(id, request, staffId);
            return Ok(new
            {
                success = true,
                message = "Notes updated successfully",
                data = delivery
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to update notes",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get delivery by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDelivery(int id)
    {
        try
        {
            var delivery = await _deliveryService.GetByIdAsync(id);
            return Ok(new
            {
                success = true,
                data = delivery
            });
        }
        catch (Exception ex)
        {
            return NotFound(new
            {
                success = false,
                message = $"Delivery {id} not found",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get delivery by GroupScheduleId
    /// </summary>
    [HttpGet("by-schedule/{groupScheduleId}")]
    public async Task<IActionResult> GetByGroupSchedule(int groupScheduleId)
    {
        try
        {
            var delivery = await _deliveryService.GetByGroupScheduleIdAsync(groupScheduleId);
            
            if (delivery == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No delivery found for GroupSchedule {groupScheduleId}"
                });
            }

            return Ok(new
            {
                success = true,
                data = delivery
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to get delivery",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [STAFF] Get own deliveries
    /// </summary>
    [HttpGet("my-deliveries")]
    public async Task<IActionResult> GetMyDeliveries()
    {
        try
        {
            int staffId = GetCurrentUserId();
            var deliveries = await _deliveryService.GetByStaffIdAsync(staffId);
            return Ok(new
            {
                success = true,
                data = deliveries
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to get deliveries",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [MANAGER/STAFF] View calendar by date range
    /// Query params: from, to, staffId (optional)
    /// </summary>
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int? staffId = null)
    {
        try
        {
            var calendar = await _deliveryService.GetCalendarAsync(from, to, staffId);
            return Ok(new
            {
                success = true,
                data = calendar
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to get calendar",
                error = ex.Message
            });
        }
    }
}