using RoboRent_BE.Service.Interface;
using Net.payOS.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoboRent_BE.Model.Entities;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace RoboRent_BE.Controllers;

[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IPayOSService _payOSService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IPaymentService _paymentService; 


    public PaymentController(IPayOSService payOSService, ILogger<PaymentController> logger, IPaymentService paymentService)
    {
        _payOSService = payOSService;
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WebhookType webhookBody)
    {
        try
        {
            _logger.LogDebug($"Webhook payload: {JsonSerializer.Serialize(webhookBody)}");

            // ✅ Verify webhook với PayOS SDK (GIỮ NGUYÊN)
            var webhookData = _payOSService.VerifyWebhookData(webhookBody);
            _logger.LogInformation($"Webhook từ PayOS: orderCode={webhookData.orderCode}, code={webhookData.code}");

            // Skip test webhook
            if (webhookData.orderCode == 123)
            {
                _logger.LogInformation("Skipping test webhook");
                return Ok(new { message = "Test webhook ignored" });
            }

            // Map PayOS code to status
            string paymentStatus = webhookData.code switch
            {
                "00" => "Paid",
                _ => "Failed"
            };

            // ✅ NEW: Delegate to PaymentService
            await _paymentService.ProcessWebhookAsync(webhookData.orderCode, paymentStatus);

            return Ok(new { message = "Webhook processed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Webhook processing error: {ex.Message}");
            return StatusCode(500, new { message = "Webhook processing failed", error = ex.Message });
        }
    }

    [HttpGet("info/{orderCode}")]
    public async Task<IActionResult> GetPaymentInfo(long orderCode)
    {
        try
        {
            var paymentInfo = await _payOSService.GetPaymentLinkInformation(orderCode);
            return Ok(paymentInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy thông tin thanh toán cho order: {orderCode}");
            return StatusCode(500, new { Message = "Lỗi khi lấy thông tin thanh toán.", Error = ex.Message });
        }
    }

    [HttpPost("cancel/{orderCode}")]
    public async Task<IActionResult> CancelPayment(long orderCode, [FromBody] string? cancellationReason = null)
    {
        try
        {
            var result = await _payOSService.CancelPaymentLink(orderCode, cancellationReason);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi hủy link thanh toán cho order: {orderCode}");
            return StatusCode(500, new { Message = "Lỗi khi hủy link thanh toán.", Error = ex.Message });
        }
    }
    
    /// <summary>
    /// [CUSTOMER] Get all payments for a rental
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
            _logger.LogError(ex, $"Error getting payments for Rental {rentalId}");
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to get payments",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// [SYSTEM/MANUAL] Manually create deposit payment (fallback if auto-create failed)
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
                message = "Deposit payment created",
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating deposit for Rental {rentalId}");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// [SYSTEM/MANUAL] Manually create full payment (fallback if auto-create failed)
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
                message = "Full payment created",
                data = payment
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating full payment for Rental {rentalId}");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}