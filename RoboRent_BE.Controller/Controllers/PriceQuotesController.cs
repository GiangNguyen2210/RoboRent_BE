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
                Content = $"📤 Staff đã tạo báo giá #{quote.QuoteNumber} và gửi lên Manager chờ duyệt",
                PriceQuoteId = quote.Id
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
    /// [MANAGER] Approve hoặc Reject báo giá
    /// </summary>
    [HttpPut("{id}/manager-action")]
    public async Task<IActionResult> ManagerAction(int id, [FromBody] ManagerActionRequest request)
    {
        try
        {
            int managerId = 2;
        
            var quote = await _priceQuoteService.ManagerActionAsync(id, request, managerId);
        
            string content = request.Action.ToLower() == "approve"
                ? $"✅ Manager đã duyệt báo giá #{quote.QuoteNumber}. Chờ Customer xác nhận."
                : $"❌ Manager từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Feedback}. Vui lòng chỉnh sửa lại.";
        
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = content,
                PriceQuoteId = quote.Id
            }, managerId);
        
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
        
            return Ok(quote);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to perform manager action", Error = ex.Message });
        }
    }
    
    /// <summary>
    /// [CUSTOMER] Approve hoặc Reject báo giá
    /// </summary>
    [HttpPut("{id}/customer-action")]
    public async Task<IActionResult> CustomerAction(int id, [FromBody] CustomerActionRequest request)
    {
        try
        {
            int customerId = 1;
        
            var quote = await _priceQuoteService.CustomerActionAsync(id, request, customerId);
        
            string content = request.Action.ToLower() == "approve"
                ? $"✅ Customer đã chấp nhận báo giá #{quote.QuoteNumber}. Tổng: ${quote.Total:N2}"
                : quote.Status == "Expired"
                    ? $"⏰ Báo giá #{quote.QuoteNumber} đã hết hạn (đã tạo đủ 3 báo giá)"
                    : $"❌ Customer từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Reason}. Vui lòng tạo báo giá mới.";
        
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = content,
                PriceQuoteId = quote.Id
            }, customerId);
        
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
        
            if (request.Action.ToLower() == "approve")
            {
                await _hubContext.Clients.Group(roomName).SendAsync("QuoteAccepted", quote.Id);
            }
        
            return Ok(new 
            { 
                Quote = quote,
                Message = request.Action.ToLower() == "approve" 
                    ? "Quote accepted successfully" 
                    : "Quote rejected successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to perform customer action", Error = ex.Message });
        }
    }
    
    /// <summary>
    /// [STAFF] Update báo giá bị Manager reject - Auto resubmit
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePriceQuote(int id, [FromBody] UpdatePriceQuoteRequest request)
    {
        try
        {
            int staffId = 1;
        
            var quote = await _priceQuoteService.UpdatePriceQuoteAsync(id, request, staffId);
        
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = $"🔄 Staff đã cập nhật báo giá #{quote.QuoteNumber} và gửi lại Manager",
                PriceQuoteId = quote.Id
            }, staffId);
        
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
        
            return Ok(quote);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to update price quote", Error = ex.Message });
        }
    }
    
}