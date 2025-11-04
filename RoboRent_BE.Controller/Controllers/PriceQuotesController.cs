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
    
    /// <summary>
    /// Customer chấp nhận báo giá
    /// Tự động reject các báo giá khác của cùng rental
    /// Gửi notification vào chat
    /// </summary>
    [HttpPut("{id}/accept")]
    public async Task<IActionResult> AcceptQuote(int id)
    {
        try
        {
            // TODO: Get customerId from authenticated user (JWT token)
            int customerId = 1; // Replace with: User.FindFirst("AccountId")?.Value
        
            // 1. Service accept quote (auto reject others)
            var quote = await _priceQuoteService.AcceptQuoteAsync(id, customerId);
        
            // 2. Send notification to chat
            var notificationMessage = await _chatService.SendMessageAsync(
                new SendMessageRequest
                {
                    RentalId = quote.RentalId,
                    MessageType = MessageType.SystemNotification,
                    Content = $"✅ Customer đã chấp nhận báo giá #{quote.QuoteNumber}. Tổng: ${quote.Total:N2}",
                    PriceQuoteId = quote.Id
                }, 
                customerId);
        
            // 3. Broadcast qua SignalR
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName)
                .SendAsync("ReceiveMessage", notificationMessage);
            await _hubContext.Clients.Group(roomName)
                .SendAsync("QuoteAccepted", quote.Id);
        
            return Ok(new 
            { 
                Quote = quote,
                Message = "Quote accepted successfully. Other pending quotes have been rejected." 
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new 
            { 
                Message = "Failed to accept quote", 
                Error = ex.Message 
            });
        }
    }
    
    /// <summary>
    /// [MANAGER] Duyệt báo giá
    /// </summary>
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveQuote(int id)
    {
        try
        {
            int managerId = 2;
            
            var quote = await _priceQuoteService.ApproveQuoteAsync(id, managerId);
            
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = $"✅ Manager đã duyệt báo giá #{quote.QuoteNumber}. Chờ Customer xác nhận.",
                PriceQuoteId = quote.Id
            }, managerId);
            
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
            
            return Ok(quote);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to approve quote", Error = ex.Message });
        }
    }

    /// <summary>
    /// [MANAGER] Từ chối báo giá
    /// </summary>
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectQuote(int id, [FromBody] ManagerRejectRequest request)
    {
        try
        {
            int managerId = 2;
            
            var quote = await _priceQuoteService.RejectQuoteAsync(id, request.Feedback, managerId);
            
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = $"❌ Manager từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Feedback}. Vui lòng chỉnh sửa lại.",
                PriceQuoteId = quote.Id
            }, managerId);
            
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
            
            return Ok(quote);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to reject quote", Error = ex.Message });
        }
    }
    
    /// <summary>
    /// [CUSTOMER] Chấp nhận báo giá
    /// </summary>
    [HttpPut("{id}/approve-quote")]
    public async Task<IActionResult> ApproveQuoteByCustomer(int id)
    {
        try
        {
            int customerId = 1;
            
            var quote = await _priceQuoteService.ApproveQuoteByCustomerAsync(id, customerId);
            
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = $"✅ Customer đã chấp nhận báo giá #{quote.QuoteNumber}. Tổng: ${quote.Total:N2}",
                PriceQuoteId = quote.Id
            }, customerId);
            
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
            await _hubContext.Clients.Group(roomName).SendAsync("QuoteAccepted", quote.Id);
            
            return Ok(new 
            { 
                Quote = quote,
                Message = "Quote accepted successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to approve quote", Error = ex.Message });
        }
    }

    /// <summary>
    /// [CUSTOMER] Từ chối báo giá
    /// </summary>
    [HttpPut("{id}/reject-quote")]
    public async Task<IActionResult> RejectQuoteByCustomer(int id, [FromBody] CustomerRejectRequest request)
    {
        try
        {
            int customerId = 1;
            
            var quote = await _priceQuoteService.RejectQuoteByCustomerAsync(id, request.Reason, customerId);
            
            var notificationMessage = await _chatService.SendMessageAsync(new SendMessageRequest
            {
                RentalId = quote.RentalId,
                MessageType = MessageType.SystemNotification,
                Content = $"❌ Customer từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Reason}. Vui lòng tạo báo giá mới.",
                PriceQuoteId = quote.Id
            }, customerId);
            
            var roomName = $"rental_{quote.RentalId}";
            await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", notificationMessage);
            
            return Ok(new 
            { 
                Quote = quote,
                Message = "Quote rejected successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to reject quote", Error = ex.Message });
        }
    }
}