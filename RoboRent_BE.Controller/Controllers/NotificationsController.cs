using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Controller.Helpers;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationsController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Get notifications for the current user with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = AuthHelper.GetCurrentUserId(User);
            var notifications = await _notificationRepository.GetByRecipientIdAsync(userId, page, pageSize);
            var totalCount = await _notificationRepository.GetTotalCountAsync(userId);
            var hasMore = (page * pageSize) < totalCount;

            return Ok(new
            {
                success = true,
                data = notifications.Select(n => new
                {
                    n.Id,
                    n.Type,
                    TypeName = n.Type.ToString(),
                    n.Content,
                    n.RentalId,
                    n.RelatedEntityId,
                    n.IsRead,
                    n.CreatedAt,
                    n.ReadAt
                }),
                page,
                pageSize,
                totalCount,
                hasMore
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get unread notification count for the current user
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = AuthHelper.GetCurrentUserId(User);
            var count = await _notificationRepository.GetUnreadCountAsync(userId);

            return Ok(new
            {
                success = true,
                unreadCount = count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Mark a specific notification as read
    /// </summary>
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _notificationRepository.MarkAsReadAsync(id);
            return Ok(new { success = true, message = "Notification marked as read" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Mark all notifications as read for the current user
    /// </summary>
    [HttpPatch("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = AuthHelper.GetCurrentUserId(User);
            await _notificationRepository.MarkAllAsReadAsync(userId);

            return Ok(new { success = true, message = "All notifications marked as read" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Soft delete a single notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        try
        {
            await _notificationRepository.SoftDeleteAsync(id);
            return Ok(new { success = true, message = "Notification deleted" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Soft delete all notifications for the current user
    /// </summary>
    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        try
        {
            var userId = AuthHelper.GetCurrentUserId(User);
            await _notificationRepository.SoftDeleteAllAsync(userId);

            return Ok(new { success = true, message = "All notifications deleted" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}

