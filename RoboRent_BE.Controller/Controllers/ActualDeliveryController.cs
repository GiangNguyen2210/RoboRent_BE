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
    /// Auto tạo delivery sau khi customer accept contract
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDelivery([FromBody] CreateActualDeliveryRequest request)
    {
        try
        {
            var delivery = await _deliveryService.CreateActualDeliveryAsync(request);
            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to create delivery", Error = ex.Message });
        }
    }

    /// <summary>
    /// [STAFF] Assign delivery và set scheduled times
    /// </summary>
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignDelivery(int id, [FromBody] AssignDeliveryRequest request)
    {
        try
        {
            int staffId = GetCurrentUserId();
            var delivery = await _deliveryService.AssignDeliveryAsync(id, request, staffId);
            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to assign delivery", Error = ex.Message });
        }
    }

    /// <summary>
    /// [STAFF] Update delivery status (Delivering → Delivered → Collecting → Collected → Completed)
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateDeliveryStatusRequest request)
    {
        try
        {
            int staffId = GetCurrentUserId();
            var delivery = await _deliveryService.UpdateStatusAsync(id, request, staffId);
            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to update status", Error = ex.Message });
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
            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = $"Delivery {id} not found", Error = ex.Message });
        }
    }

    /// <summary>
    /// [CUSTOMER] Track delivery by rental ID
    /// </summary>
    [HttpGet("rental/{rentalId}")]
    public async Task<IActionResult> GetByRentalId(int rentalId)
    {
        try
        {
            var delivery = await _deliveryService.GetByRentalIdAsync(rentalId);
            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = $"No delivery found for rental {rentalId}", Error = ex.Message });
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
            return Ok(deliveries);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get deliveries", Error = ex.Message });
        }
    }

    /// <summary>
    /// [STAFF] View calendar by date range
    /// Query params: from, to, staffId (optional - nếu không truyền thì lấy tất cả)
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
            return Ok(calendar);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get calendar", Error = ex.Message });
        }
    }
}