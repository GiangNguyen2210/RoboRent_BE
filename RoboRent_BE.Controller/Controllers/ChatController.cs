using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RoboRent_BE.Model.DTOs.Chat;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Controller.Hubs;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
    {
        _chatService = chatService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Tạo hoặc lấy chat room cho rental
    /// </summary>
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateOrGetChatRoom(
        [FromQuery] int rentalId,
        [FromQuery] int staffId,
        [FromQuery] int customerId)
    {
        try
        {
            var chatRoom = await _chatService.GetOrCreateChatRoomAsync(rentalId, staffId, customerId);
            return Ok(chatRoom);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to create chat room", Error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chat room theo rentalId
    /// </summary>
    [HttpGet("rooms/{rentalId}")]
    public async Task<IActionResult> GetChatRoom(int rentalId)
    {
        try
        {
            var chatRoom = await _chatService.GetChatRoomByRentalIdAsync(rentalId);
            return Ok(chatRoom);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = $"Chat room not found for rental {rentalId}", Error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy messages của chat room (có pagination)
    /// </summary>
    [HttpGet("messages/{rentalId}")]
    public async Task<IActionResult> GetMessages(
        int rentalId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var messages = await _chatService.GetChatMessagesAsync(rentalId, page, pageSize);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get messages", Error = ex.Message });
        }
    }

    /// <summary>
    /// Gửi message (text, demo, hoặc notification)
    /// </summary>
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            // TODO: Get senderId from authenticated user (JWT token)
            int senderId = 1; // Replace with: User.FindFirst("AccountId")?.Value
            
            // 1. Service lưu message vào DB
            var message = await _chatService.SendMessageAsync(request, senderId);
            
            // 2. Controller broadcast qua SignalR
            var roomName = $"rental_{request.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", message);
            
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to send message", Error = ex.Message });
        }
    }

    /// <summary>
    /// Customer accept/reject demo
    /// </summary>
    [HttpPut("messages/{messageId}/status")]
    public async Task<IActionResult> UpdateMessageStatus(
        int messageId,
        [FromBody] UpdateMessageStatusRequest request)
    {
        try
        {
            // 1. Service update status trong DB
            var message = await _chatService.UpdateMessageStatusAsync(messageId, request);
            
            // 2. Controller broadcast status change qua SignalR
            var chatRoom = await _chatService.GetChatRoomByRentalIdAsync(message.ChatRoomId);
            var roomName = $"rental_{message.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("DemoStatusChanged", messageId, request.Status);
            
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to update message status", Error = ex.Message });
        }
    }

    /// <summary>
    /// Đếm số tin nhắn chưa đọc
    /// </summary>
    [HttpGet("unread-count/{rentalId}")]
    public async Task<IActionResult> GetUnreadCount(int rentalId)
    {
        try
        {
            // TODO: Get userId from authenticated user
            int userId = 1; // Replace with authenticated userId
            
            var count = await _chatService.GetUnreadCountAsync(rentalId, userId);
            return Ok(new { RentalId = rentalId, UnreadCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get unread count", Error = ex.Message });
        }
    }
}