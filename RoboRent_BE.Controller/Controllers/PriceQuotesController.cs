using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.PriceQuote;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Controller.Helpers;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PriceQuotesController : ControllerBase
{
    private readonly IPriceQuoteService _priceQuoteService;
    private readonly INotificationService _notificationService;
    private readonly IRentalService _rentalService;
    private readonly IAccountService _accountService;

    public PriceQuotesController(
        IPriceQuoteService priceQuoteService,
        INotificationService notificationService,
        IRentalService rentalService,
        IAccountService accountService)
    {
        _priceQuoteService = priceQuoteService;
        _notificationService = notificationService;
        _rentalService = rentalService;
        _accountService = accountService;
    }

    /// <summary>
    /// Tạo báo giá mới (max 3 lần)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePriceQuote([FromBody] CreatePriceQuoteRequest request)
    {
        try
        {
            int staffId = AuthHelper.GetCurrentUserId(User);

            var quote = await _priceQuoteService.CreatePriceQuoteAsync(request, staffId);

            // 🔔 Notify ALL Managers: Quote pending approval
            var managers = await _accountService.GetAllManagerAccountsAsync();
            if (managers != null && managers.Any())
            {
                await _notificationService.CreateNotificationsAsync(
                    managers.Select(m => m.Id),
                    NotificationType.QuotePendingApproval,
                    $"📤 Báo giá #{quote.QuoteNumber} đang chờ duyệt.",
                    request.RentalId,
                    quote.Id,
                    isRealTime: true);
            }

            return Ok(quote);
        }
        catch (Exception ex)
        {
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
            var rental = await _rentalService.GetRentalAsync(quote.RentalId);

            if (request.Action.ToLower() == "approve")
            {
                // 🔔 Notify Customer: New quote ready for review (don't mention manager)
                if (rental?.AccountId != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        rental.AccountId.Value,
                        NotificationType.QuoteApproved,
                        $"📋 Bạn có báo giá mới #{quote.QuoteNumber}. Vui lòng xem và xác nhận.",
                        quote.RentalId,
                        quote.Id,
                        isRealTime: true);
                }
            }
            else
            {
                // 🔔 Notify Staff: Quote rejected by manager
                if (rental?.StaffId != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        rental.StaffId.Value,
                        NotificationType.QuoteRejected,
                        $"❌ Báo giá #{quote.QuoteNumber} bị từ chối. Lý do: {request.Feedback}",
                        quote.RentalId,
                        quote.Id,
                        isRealTime: true);
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
            var rental = await _rentalService.GetRentalAsync(quote.RentalId);

            if (request.Action.ToLower() == "approve")
            {
                // 🔔 Notify Staff: Quote accepted by customer
                if (rental?.StaffId != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        rental.StaffId.Value,
                        NotificationType.QuoteAccepted,
                        $"✅ Khách hàng đã chấp nhận báo giá #{quote.QuoteNumber}. Tổng: {quote.GrandTotal:N0}₫",
                        quote.RentalId,
                        quote.Id,
                        isRealTime: true);
                }
            }
            else
            {
                // 🔔 Notify Staff: Quote rejected by customer
                if (rental?.StaffId != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        rental.StaffId.Value,
                        NotificationType.QuoteRejectedByCustomer,
                        $"❌ Khách hàng từ chối báo giá #{quote.QuoteNumber}. Lý do: {request.Reason}",
                        quote.RentalId,
                        quote.Id,
                        isRealTime: true);
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

            // 🔔 Notify ALL Managers: Quote updated and resubmitted
            var managers = await _accountService.GetAllManagerAccountsAsync();
            if (managers != null && managers.Any())
            {
                await _notificationService.CreateNotificationsAsync(
                    managers.Select(m => m.Id),
                    NotificationType.QuotePendingApproval,
                    $"🔄 Báo giá #{quote.QuoteNumber} đã được cập nhật và gửi lại.",
                    quote.RentalId,
                    quote.Id,
                    isRealTime: true);
            }

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

    /// <summary>
    /// [STAFF] Reject active quotes for rental when rental info is updated
    /// </summary>
    [HttpPost("reject-active-for-rental/{rentalId}")]
    public async Task<IActionResult> RejectActiveQuotesForRental(int rentalId)
    {
        try
        {
            var rejectedCount = await _priceQuoteService.RejectActiveQuotesForRentalAsync(rentalId);
            
            return Ok(new
            {
                success = true,
                message = $"Rejected {rejectedCount} active quote(s) for rental {rentalId}",
                rejectedCount = rejectedCount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to reject active quotes", Error = ex.Message });
        }
    }
}