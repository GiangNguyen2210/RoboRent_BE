using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.GroupSchedule;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class GroupScheduleController : ControllerBase
{
    private readonly IGroupScheduleService _groupScheduleService;
    private readonly INotificationService _notificationService;

    public GroupScheduleController(
        IGroupScheduleService groupScheduleService,
        INotificationService notificationService)
    {
        _groupScheduleService = groupScheduleService;
        _notificationService = notificationService;
    }

    [HttpGet("staff/get/all/schedules/{groupId}")]
    public async Task<IActionResult> GetGroupSchedulesById(int groupId)
    {
        try
        {
            var res = await _groupScheduleService.GetGroupScheduleByGroupId(groupId);

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

    [HttpPost("staff/add/schedule/{staffId}")]
    public async Task<IActionResult> AddGroupSchedule([FromBody] GroupScheduleCreateRequest request, int staffId)
    {
        try
        {
            var res = await _groupScheduleService.CreateGroupSchedule(request, staffId);

            if (res == null)
            {
                return NotFound(new
                {
                    success = false,
                    messsage = "TypeActivityGroup or Rental could not be found."
                });
            }

            // 🔔 Notify Customer about new schedule
            if (res.CustomerId.HasValue && res.RentalId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    res.CustomerId.Value,
                    NotificationType.ScheduleCreated,
                    $"📅 Lịch trình đã được tạo cho sự kiện ngày {res.EventDate:dd/MM/yyyy}. Vui lòng kiểm tra.",
                    res.RentalId.Value,
                    res.Id,
                    isRealTime: true);
            }

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

    [HttpPut("staff/update/{scheduleId}")]
    public async Task<IActionResult> UpdateGroupSchedule(int scheduleId, [FromBody] GroupScheduleUpdateRequest request)
    {
        try
        {
            var res = await _groupScheduleService.UpdateGroupSchedule(scheduleId, request);

            if (res == null)
            {
                return NotFound(new
                {
                    success = false,
                    messsage = "TypeActivityGroup/Rental/Schedule could not be found."
                });
            }

            // 🔔 Notify Customer about schedule update
            if (res.CustomerId.HasValue && res.RentalId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    res.CustomerId.Value,
                    NotificationType.ScheduleUpdated,
                    $"📅 Lịch trình ngày {res.EventDate:dd/MM/yyyy} đã được cập nhật. Vui lòng kiểm tra.",
                    res.RentalId.Value,
                    res.Id,
                    isRealTime: true);
            }

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

    [HttpDelete("staff/cancel/schedule/{scheduleId}")]
    public async Task<IActionResult> CancelScheduleById(int scheduleId)
    {
        try
        {
            var res = await _groupScheduleService.CancelGroupScheduleById(scheduleId);

            if (res == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Group schedule could not be found."
                });
            }

            // 🔔 Notify Customer about schedule cancellation
            if (res.CustomerId.HasValue && res.RentalId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    res.CustomerId.Value,
                    NotificationType.ScheduleCancelled,
                    $"📅 Lịch trình ngày {res.EventDate:dd/MM/yyyy} đã bị hủy. Vui lòng liên hệ nhân viên.",
                    res.RentalId.Value,
                    res.Id,
                    isRealTime: true);
            }

            return Ok();
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


    [HttpGet("customer/get/schedule/{rentalId}")]
    public async Task<IActionResult> GetGroupSchedulesByRentalId(int rentalId)
    {
        try
        {
            var res = await _groupScheduleService.CustomerGetGroupScheduleByRentalId(rentalId);

            if (res == null) return NotFound(new
            {
                success = false,
                message = "No Schedule Found"
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