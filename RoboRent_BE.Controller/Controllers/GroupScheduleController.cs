using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.GroupSchedule;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class GroupScheduleController : ControllerBase
{
    private readonly IGroupScheduleService _groupScheduleService;

    public GroupScheduleController(IGroupScheduleService groupScheduleService)
    {
        _groupScheduleService = groupScheduleService;
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
    public async Task<IActionResult> UpdateGroupSchedule(int  scheduleId, [FromBody] GroupScheduleUpdateRequest request)
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
    
}