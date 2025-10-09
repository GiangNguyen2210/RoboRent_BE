﻿using RoboRent_BE.Service.Interface;
using Net.payOS.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoboRent_BE.Model.Entities;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOS;

namespace RoboRent_BE.Controllers;

[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPayOSService _payOSService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPayOSService payOSService, ILogger<PaymentController> logger)
    {
        _payOSService = payOSService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request, [FromQuery] int accountId)
    {
        try
        {
            _logger.LogInformation($"Received payment request for order: {request?.OrderCode}, amount: {request?.Amount}, accountId: {accountId}");

            if (accountId <= 0)
            {
                return BadRequest(new { message = "Yêu cầu AccountId hợp lệ." });
            }

            if (request == null || request.Amount <= 0 || string.IsNullOrEmpty(request.Description))
            {
                return BadRequest(new { message = "Dữ liệu yêu cầu thanh toán không hợp lệ." });
            }

            var accountDto = await _payOSService.GetAccountByIdAsync(accountId);
            if (accountDto == null)
            {
                _logger.LogWarning($"Không tìm thấy Account với ID {accountId}.");
                return BadRequest(new { message = "Account không hợp lệ." });
            }

            var response = await _payOSService.CreatePaymentLink(request, accountId);
            if (response.Code != "PENDING")
            {
                _logger.LogError($"Tạo link thanh toán thất bại: {response.Desc}");
                return BadRequest(new { message = response.Desc });
            }

            _logger.LogInformation($"Đã tạo link thanh toán: {response.Data.CheckoutUrl}");
            return Ok(new { checkoutUrl = response.Data.CheckoutUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Tạo link thanh toán thất bại: {ex.Message}");
            return StatusCode(500, new { Message = "Tạo link thanh toán thất bại. Vui lòng thử lại sau.", Error = ex.Message });
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WebhookType webhookBody)
    {
        try
        {
            _logger.LogDebug($"Webhook payload: {JsonSerializer.Serialize(webhookBody)}");

            var webhookData = _payOSService.VerifyWebhookData(webhookBody);
            _logger.LogInformation($"Nhận webhook từ PayOS: orderCode={webhookData.orderCode}, code={webhookData.code}, desc={webhookData.desc}");

            if (webhookData.orderCode == 123)
            {
                _logger.LogInformation("Bỏ qua webhook test với orderCode 123.");
                return Ok(new { message = "Webhook test được bỏ qua." });
            }

            var transactionDto = await _payOSService.GetTransactionByOrderCodeAsync(webhookData.orderCode);
            if (transactionDto == null)
            {
                _logger.LogWarning($"Không tìm thấy giao dịch với orderCode {webhookData.orderCode}.");
                return BadRequest(new { message = "Không tìm thấy giao dịch." });
            }

            string transactionStatus = webhookData.code switch
            {
                "00" => "PAID",
                _ => "FAILED"
            };

            if (transactionDto.Status == transactionStatus)
            {
                _logger.LogInformation($"Giao dịch {webhookData.orderCode} đã được xử lý với trạng thái {transactionStatus}.");
                return Ok(new { message = "Webhook đã được xử lý." });
            }

            await _payOSService.UpdateTransactionStatusAsync(webhookData.orderCode, transactionStatus);

            if (transactionStatus == "PAID")
            {
                await _payOSService.UpdateAccountSubscriptionAsync(transactionDto.AccountId, transactionDto.Amount);
            }

            return Ok(new { message = "Webhook được xử lý thành công." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi xử lý webhook: {ex.Message}");
            return StatusCode(500, new { Message = "Lỗi khi xử lý webhook.", Error = ex.Message });
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

    [HttpGet("info/user/{accountId}")]
    public async Task<IActionResult> GetPaymentInfoByAccountId(int accountId)
    {
        try
        {
            return Ok(await _payOSService.GetPaymentTransactionByAccountId(accountId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy thông tin thanh toán cho AccountId: {accountId}");
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
}