using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RoboRent_BE.Model.DTOs.Chat;
using RoboRent_BE.Model.DTOs.PriceQuote;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Controller.Hubs;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PriceQuotesController : ControllerBase
{
    private readonly IPriceQuoteService _priceQuoteService;
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hubContext;

    public PriceQuotesController(
        IPriceQuoteService priceQuoteService,
        IChatService chatService,
        IHubContext<ChatHub> hubContext)
    {
        _priceQuoteService = priceQuoteService;
        _chatService = chatService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Tạo báo giá mới (max 3 lần)
    /// Auto gửi notification vào chat
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePriceQuote([FromBody] CreatePriceQuoteRequest request)
    {
        try
        {
            // TODO: Get staffId from authenticated user (JWT token)
            int staffId = 1; // Replace with: User.FindFirst("AccountId")?.Value
            
            // 1. Service tạo quote (check < 3)
            var quote = await _priceQuoteService.CreatePriceQuoteAsync(request, staffId);
            
            // 2. Controller gửi notification vào chat
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = request.RentalId,
                MessageType = MessageType.PriceQuoteNotification,
                Content = $"Staff đã tạo báo giá #{quote.QuoteNumber}",
                RelatedEntityId = quote.Id
            }, staffId);
            
            // 3. Broadcast notification qua SignalR
            var roomName = $"rental_{request.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
            
            return Ok(quote);
        }
        catch (Exception ex)
        {
            // Check if error is about max quotes
            if (ex.Message.Contains("Maximum 3 quotes"))
            {
                return BadRequest(new 
                { 
                    Message = "Không thể tạo thêm báo giá", 
                    Error = "Đã đạt giới hạn 3 lần chỉnh sửa báo giá cho rental này",
                    MaxQuotesReached = true
                });
            }
            
            return BadRequest(new { Message = "Failed to create price quote", Error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết 1 báo giá
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPriceQuote(int id)
    {
        try
        {
            var quote = await _priceQuoteService.GetPriceQuoteAsync(id);
            return Ok(quote);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = $"Price quote {id} not found", Error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy tất cả báo giá của 1 rental
    /// Response bao gồm: danh sách quotes, tổng số quotes, và flag CanCreateMore
    /// </summary>
    [HttpGet("rental/{rentalId}")]
    public async Task<IActionResult> GetQuotesByRentalId(int rentalId)
    {
        try
        {
            var quotes = await _priceQuoteService.GetQuotesByRentalIdAsync(rentalId);
            return Ok(quotes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get quotes", Error = ex.Message });
        }
    }

    /// <summary>
    /// Check xem có thể tạo thêm quote không
    /// </summary>
    [HttpGet("rental/{rentalId}/can-create")]
    public async Task<IActionResult> CanCreateMoreQuotes(int rentalId)
    {
        try
        {
            var quotes = await _priceQuoteService.GetQuotesByRentalIdAsync(rentalId);
            return Ok(new 
            { 
                RentalId = rentalId,
                CanCreateMore = quotes.CanCreateMore,
                CurrentQuoteCount = quotes.TotalQuotes,
                MaxQuotes = 3
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to check quote limit", Error = ex.Message });
        }
    }
}