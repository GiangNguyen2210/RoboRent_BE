using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Controller.Helpers;
using RoboRent_BE.Model.DTOs.ActualDelivery;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActualDeliveryController : ControllerBase
{
    private readonly IActualDeliveryService _deliveryService;
    private readonly INotificationService _notificationService;

    public ActualDeliveryController(
        IActualDeliveryService deliveryService,
        INotificationService notificationService)
    {
        _deliveryService = deliveryService;
        _notificationService = notificationService;
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
            return Ok(delivery);  // ✅ Trả trực tiếp
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to create delivery", Error = ex.Message });
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

            // 🔔 Notify assigned Staff Tech
            if (delivery.StaffId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    delivery.StaffId.Value,
                    NotificationType.DeliveryAssigned,
                    $"🚚 Bạn được phân công giao thiết bị cho sự kiện ngày {delivery.ScheduleInfo?.EventDate:dd/MM/yyyy}.",
                    delivery.RentalInfo?.RentalId ?? 0,
                    delivery.Id,
                    isRealTime: true);
            }

            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to assign staff", Error = ex.Message });
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
            return Ok(result);  // ✅
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to check conflict", Error = ex.Message });
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
            int staffId = AuthHelper.GetCurrentUserId(User);
            var delivery = await _deliveryService.UpdateStatusAsync(id, request, staffId);
            return Ok(delivery);  // ✅
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to update status", Error = ex.Message });
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
            int staffId = AuthHelper.GetCurrentUserId(User);
            var delivery = await _deliveryService.UpdateNotesAsync(id, request, staffId);
            return Ok(delivery);  // ✅
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to update notes", Error = ex.Message });
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
            return Ok(delivery);  // ✅
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = $"Delivery {id} not found", Error = ex.Message });
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
                return NotFound(new { Message = $"No delivery found for GroupSchedule {groupScheduleId}" });
            }

            return Ok(delivery);  // ✅
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get delivery", Error = ex.Message });
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
            int staffId = AuthHelper.GetCurrentUserId(User);
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
            return Ok(calendar);  // ✅
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get calendar", Error = ex.Message });
        }
    }

    /// <summary>
    /// [MANAGER] Get all pending deliveries for staff assignment
    /// Query params: page, pageSize, searchTerm, sortBy (date|name|customer|location)
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingDeliveries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "date")
    {
        try
        {
            var result = await _deliveryService.GetPendingDeliveriesAsync(
                page,
                pageSize,
                searchTerm,
                sortBy
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new {
                Message = "Failed to get pending deliveries",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// [MANAGER] Assign staff for ALL pending deliveries in (EventDate + ActivityTypeGroup)
    /// </summary>
    [HttpPut("assign-staff-batch")]
    public async Task<IActionResult> AssignStaffBatch([FromBody] AssignStaffBatchRequest request)
    {
        try
        {
            // Convert EventDate to UTC for PostgreSQL compatibility
            var requestWithUtc = new AssignStaffBatchRequest
            {
                ActivityTypeGroupId = request.ActivityTypeGroupId,
                EventDate = DateTime.SpecifyKind(request.EventDate, DateTimeKind.Utc),
                StaffId = request.StaffId,
                Notes = request.Notes,
                ForcePartialAssign = request.ForcePartialAssign
            };
            
            var response = await _deliveryService.AssignStaffBatchAsync(requestWithUtc);

            // If conflict detected and not forced, return conflict info for confirmation
            if (response.HasConflict && !response.Success)
            {
                return Ok(new
                {
                    success = false,
                    hasConflict = true,
                    conflictMessage = response.ConflictMessage,
                    conflictingScheduleIds = response.ConflictingScheduleIds,
                    assignedCount = 0
                });
            }

            // Success (notification removed - not critical for batch assignment)
            
            return Ok(new
            {
                success = true,
                assignedCount = response.AssignedCount,
                hasConflict = response.HasConflict,
                message = response.ConflictMessage ?? $"Đã assign staff cho {response.AssignedCount} deliveries"
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
    /// [MANAGER] Get pending deliveries grouped by (EventDate + ActivityTypeGroup)
    /// Query params: from, to (optional)
    /// </summary>
    [HttpGet("pending-grouped")]
    public async Task<IActionResult> GetPendingDeliveriesGrouped(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        try
        {
            // Convert to UTC for PostgreSQL compatibility
            DateTime? fromUtc = from.HasValue ? DateTime.SpecifyKind(from.Value, DateTimeKind.Utc) : null;
            DateTime? toUtc = to.HasValue ? DateTime.SpecifyKind(to.Value, DateTimeKind.Utc) : null;
            
            var result = await _deliveryService.GetPendingDeliveriesGroupedAsync(fromUtc, toUtc);
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
                message = "Failed to get pending deliveries grouped",
                error = ex.Message
            });
        }
    }
    
    /// <summary>
    /// [CUSTOMER] Get delivery info by rentalId
    /// Returns first delivery found for this rental
    /// </summary>
    [HttpGet("by-rental/{rentalId}")]
    public async Task<IActionResult> GetByRentalId(int rentalId)
    {
        try
        {
            var delivery = await _deliveryService.GetByRentalIdAsync(rentalId);
        
            if (delivery == null)
            {
                return NotFound(new { Message = $"No delivery found for Rental {rentalId}" });
            }

            return Ok(delivery);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get delivery", Error = ex.Message });
        }
    }
    
}