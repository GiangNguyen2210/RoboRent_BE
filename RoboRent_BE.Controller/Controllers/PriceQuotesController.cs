using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RoboRent_BE.Model.DTOs.Chat;
using RoboRent_BE.Model.DTOs.PriceQuote;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Controller.Hubs;
using RoboRent_BE.Controller.Helpers;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PriceQuotesController : ControllerBase
{
    private readonly IPriceQuoteService _priceQuoteService;
    private readonly ChatNotificationHelper _notificationHelper;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IRentalService _rentalService;

    public PriceQuotesController(
        IPriceQuoteService priceQuoteService,
        ChatNotificationHelper notificationHelper,
        IHubContext<ChatHub> hubContext,
        IRentalService rentalService)
    {
        _priceQuoteService = priceQuoteService;
        _notificationHelper = notificationHelper;
        _hubContext = hubContext;
        _rentalService = rentalService;
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
            int staffId = AuthHelper.GetCurrentUserId(User);

            // 1. Service tạo quote (check < 3)
            var quote = await _priceQuoteService.CreatePriceQuoteAsync(request, staffId);
            var rental = await _rentalService.GetRentalAsync(request.RentalId);

            // 2. Gửi notification vào chat + broadcast qua SignalR
            await _notificationHelper.SendNotificationAsync(
                request.RentalId,
                staffId,
                $"📤 Staff đã tạo báo giá #{quote.QuoteNumber} và gửi lên Manager chờ duyệt",
                quote.Id,
                null,
                MessageType.PriceQuoteNotification
            );


            // 🎯 REFACTORED: Targeted broadcast QuoteCreated
            // Chỉ gửi đến Customer (Staff là người tạo, không cần notification!)
            if (rental != null && rental.AccountId.HasValue)
            {
                var customerId = rental.AccountId.Value.ToString();
                await _hubContext.Clients.User(customerId).SendAsync("QuoteCreated", new
                {
                    QuoteId = quote.Id,
                    QuoteNumber = quote.QuoteNumber,
                    Total = quote.GrandTotal
                });
            }

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
            int managerId = AuthHelper.GetCurrentUserId(User);

            var quote = await _priceQuoteService.ManagerActionAsync(id, request, managerId);

            string content = request.Action.ToLower() == "approve"
                ? $"✅ Manager đã duyệt báo giá #{quote.QuoteNumber}. Chờ Customer xác nhận."
                : $"❌ Manager từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Feedback}. Vui lòng chỉnh sửa lại.";

            await _notificationHelper.SendNotificationAsync(
            quote.RentalId,
            managerId,
            content,
            quote.Id
        );

            // 🎯 REFACTORED: Targeted SignalR broadcast
            // Load Rental để lấy CustomerId và StaffId
            var rental = await _rentalService.GetRentalAsync(quote.RentalId);
            if (rental != null)
            {
                if (quote.Status == "Approved")
                {
                    // Approved: Gửi đến Staff + Customer (Manager là người approve, không nhận)
                    var staffId = rental.StaffId?.ToString() ?? "";
                    var customerId = rental.AccountId?.ToString() ?? "";
                    var recipients = new string[] { staffId, customerId }.Where(id => !string.IsNullOrEmpty(id)).ToArray();
                    await _hubContext.Clients.Users(recipients).SendAsync("QuoteStatusChanged", new
                    {
                        QuoteId = quote.Id,
                        Status = quote.Status,
                        QuoteNumber = quote.QuoteNumber,
                        Total = quote.GrandTotal
                    });
                }
                else if (quote.Status == "Rejected")
                {
                    // Rejected: Chỉ gửi đến Staff (Customer không cần biết Manager reject)
                    var staffId = rental.StaffId.ToString();
                    await _hubContext.Clients.User(staffId).SendAsync("QuoteStatusChanged", new
                    {
                        QuoteId = quote.Id,
                        Status = quote.Status,
                        QuoteNumber = quote.QuoteNumber,
                        Total = quote.GrandTotal
                    });
                }
            }

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
            int customerId = AuthHelper.GetCurrentUserId(User);

            var quote = await _priceQuoteService.CustomerActionAsync(id, request, customerId);

            string content = request.Action.ToLower() == "approve"
                ? $"✅ Customer đã chấp nhận báo giá #{quote.QuoteNumber}. Tổng: ${quote.GrandTotal:N2}"
                : quote.Status == "Expired"
                    ? $"⏰ Báo giá #{quote.QuoteNumber} đã hết hạn (đã tạo đủ 3 báo giá)"
                    : $"❌ Customer từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Reason}. Vui lòng tạo báo giá mới.";

            await _notificationHelper.SendNotificationAsync(
                quote.RentalId,
                customerId,
                content,
                quote.Id
            );

            // 🎯 REFACTORED: Targeted SignalR broadcasts
            var rental = await _rentalService.GetRentalAsync(quote.RentalId);
            if (rental != null && rental.StaffId.HasValue)
            {
                var staffId = rental.StaffId.Value.ToString();

                if (request.Action.ToLower() == "approve")
                {
                    // QuoteAccepted: Chỉ gửi đến Staff (Customer là người accept, không cần notification!)
                    await _hubContext.Clients.User(staffId).SendAsync("QuoteAccepted", quote.Id);
                }
                else if (request.Action.ToLower() == "reject")
                {
                    // 🔴 NEW: QuoteRejected event (FIX BUG #1!)
                    // Chỉ gửi đến Staff (Customer là người reject, không cần notification!)
                    await _hubContext.Clients.User(staffId).SendAsync("QuoteRejected", new
                    {
                        QuoteId = quote.Id,
                        QuoteNumber = quote.QuoteNumber,
                        Reason = request.Reason
                    });
                }
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
            int staffId = AuthHelper.GetCurrentUserId(User);

            var quote = await _priceQuoteService.UpdatePriceQuoteAsync(id, request, staffId);

            await _notificationHelper.SendNotificationAsync(
                quote.RentalId,
                staffId,
                $"🔄 Staff đã cập nhật báo giá #{quote.QuoteNumber} và gửi lại Manager",
                quote.Id
            );

            return Ok(quote);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to update price quote", Error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuotes([FromQuery] string? status = null)
    {
        try
        {
            var quotes = await _priceQuoteService.GetAllQuotesForManagerAsync(status);
            return Ok(quotes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get quotes", Error = ex.Message });
        }
    }
}