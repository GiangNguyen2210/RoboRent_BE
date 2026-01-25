using RoboRent_BE.Service.Interface;
using Net.payOS.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text.Json;
using RoboRent_BE.Model.DTOS;
using Microsoft.AspNetCore.Authorization;
using RoboRent_BE.Controller.Helpers;

namespace RoboRent_BE.Controllers;

[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IPaymentService _paymentService;

    public PaymentController(
        ILogger<PaymentController> logger, 
        IPaymentService paymentService)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// [WEBHOOK] PayOS webhook endpoint for payment status updates
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WebhookType webhookBody)
    {
        try
        {
            _logger.LogDebug($"📨 Webhook received: {JsonSerializer.Serialize(webhookBody)}");

            // ✅ Verify webhook signature
            WebhookData webhookData;
            try
            {
                webhookData = _paymentService.VerifyPaymentWebhook(webhookBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"⚠️ Invalid webhook signature: {ex.Message}");
                return Unauthorized(new { message = "Invalid webhook signature" });
            }

            _logger.LogInformation($"✅ Webhook verified - OrderCode: {webhookData.orderCode}, Code: {webhookData.code}");

            // Skip test webhook from PayOS
            if (webhookData.orderCode == 123)
            {
                _logger.LogInformation("⏭️ Skipping test webhook");
                return Ok(new { message = "Test webhook ignored" });
            }

            // Map PayOS code to payment status
            string paymentStatus = webhookData.code switch
            {
                "00" => "Paid",
                "01" => "Failed",
                "02" => "Processing",
                "03" => "Cancelled",
                _ => "Unknown"
            };

            if (paymentStatus == "Unknown")
            {
                _logger.LogWarning($"⚠️ Unknown payment status code: {webhookData.code} for OrderCode {webhookData.orderCode}");
            }

            // Process business logic
            await _paymentService.ProcessWebhookAsync(webhookData.orderCode, paymentStatus);

            return Ok(new 
            { 
                success = true,
                message = "Webhook processed successfully",
                orderCode = webhookData.orderCode,
                status = paymentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Webhook processing error: {ex.Message}");
            return StatusCode(500, new 
            { 
                success = false,
                message = "Webhook processing failed", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// [PUBLIC] Get payment information from PayOS by orderCode
    /// </summary>
    [HttpGet("info/{orderCode}")]
    public async Task<IActionResult> GetPaymentInfo(long orderCode)
    {
        try
        {
            var paymentInfo = await _paymentService.GetPaymentLinkInformationAsync(orderCode);
            return Ok(new
            {
                success = true,
                data = paymentInfo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error getting payment info for OrderCode: {orderCode}");
            return StatusCode(500, new 
            { 
                success = false,
                message = "Failed to get payment information", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// [ADMIN] Cancel payment link in PayOS
    /// </summary>
    [HttpPost("cancel/{orderCode}")]
    public async Task<IActionResult> CancelPayment(long orderCode, [FromBody] string? cancellationReason = null)
    {
        try
        {
            var result = await _paymentService.CancelPaymentLinkAsync(orderCode, cancellationReason);
            return Ok(new
            {
                success = true,
                message = "Payment link cancelled",
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error cancelling payment for OrderCode: {orderCode}");
            return StatusCode(500, new 
            { 
                success = false,
                message = "Failed to cancel payment link", 
                error = ex.Message 
            });
        }
    }
    
    /// <summary>
    /// [CUSTOMER] Get all payment records for a rental
    /// </summary>
    [HttpGet("rental/{rentalId}")]
    public async Task<IActionResult> GetPaymentsByRental(int rentalId)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByRentalIdAsync(rentalId);
            return Ok(new
            {
                success = true,
                data = payments
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error getting payments for Rental {rentalId}");
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to get payments",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [SYSTEM/ADMIN] Manually create deposit payment (fallback if auto-create failed)
    /// </summary>
    [HttpPost("create-deposit/{rentalId}")]
    public async Task<IActionResult> CreateDepositPayment(int rentalId)
    {
        try
        {
            var payment = await _paymentService.CreateDepositPaymentAsync(rentalId);
            return Ok(new
            {
                success = true,
                message = "Deposit payment created successfully",
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error creating deposit for Rental {rentalId}: {ex.Message}");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// [SYSTEM/ADMIN] Manually create full payment (fallback if auto-create failed)
    /// </summary>
    [HttpPost("create-full/{rentalId}")]
    public async Task<IActionResult> CreateFullPayment(int rentalId)
    {
        try
        {
            var payment = await _paymentService.CreateFullPaymentAsync(rentalId);
            return Ok(new
            {
                success = true,
                message = "Full payment created successfully",
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error creating full payment for Rental {rentalId}: {ex.Message}");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
    
    /// <summary>
    /// [CUSTOMER] Get ALL my payment transactions (For Transaction History Page)
    /// </summary>
    [HttpGet("my-transactions")]
    [Authorize]
    public async Task<IActionResult> GetMyTransactions()
    {
        try
        {
            int userId = AuthHelper.GetCurrentUserId(User);
            var transactions = await _paymentService.GetCustomerTransactionsAsync(userId);
        
            return Ok(new
            {
                success = true,
                data = transactions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error getting transactions for User {AuthHelper.GetCurrentUserId(User)}: {ex.Message}");
        
            return StatusCode(500, new 
            { 
                success = false, 
                message = "Failed to get customer transactions",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [DEV/TESTING] Manually simulate webhook payment completion for localhost testing
    /// Mimics exactly what ProcessWebhookAsync does:
    /// 1. Updates PaymentRecord (Status, PaidAt)
    /// 2. If Deposit + Paid → Creates ActualDelivery + Checklist
    /// </summary>
    [HttpPost("dev-complete-payment/{orderCode}")]
    public async Task<IActionResult> DevCompletePayment(long orderCode)
    {
        try
        {
            _logger.LogInformation($"🛠️ [DEV] Manual payment completion triggered for OrderCode: {orderCode}");

            // Reuse the exact same logic as webhook
            await _paymentService.ProcessWebhookAsync(orderCode, "Paid");

            return Ok(new
            {
                success = true,
                message = "Payment manually marked as Paid (dev mode)",
                orderCode = orderCode,
                note = "ActualDelivery and Checklist created if this was a Deposit payment"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ [DEV] Error processing manual payment for OrderCode {orderCode}: {ex.Message}");
            return StatusCode(500, new
            {
                success = false,
                message = "Manual payment processing failed",
                error = ex.Message
            });
        }
    }

    [HttpPut("mark-as-paid/{orderCode}")]
    public async Task<IActionResult> MarkPaymentAsPaid(long orderCode)
    {
        try
        {
            await _paymentService.ProcessWebhookAsync(orderCode, "Paid");
            return Ok(new
            {
                success = true,
                message = "Payment marked as Paid",
                orderCode = orderCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking payment as Paid for OrderCode {orderCode}: {ex.Message}");
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to mark as Paid",
                error = ex.Message
            });
        }
    }
}