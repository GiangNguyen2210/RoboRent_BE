using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;
using System.Security.Claims;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContractDraftsController : ControllerBase
{
    private readonly IContractDraftsService _contractDraftsService;
    private readonly INotificationService _notificationService;
    private readonly IRentalService _rentalService;

    public ContractDraftsController(
        IContractDraftsService contractDraftsService,
        INotificationService notificationService,
        IRentalService rentalService)
    {
        _contractDraftsService = contractDraftsService;
        _notificationService = notificationService;
        _rentalService = rentalService;
    }

    [HttpGet]
    [Authorize(Roles = "Manager, Staff, Admin, Customer")]
    public async Task<IActionResult> GetAllContractDrafts()
    {
        try
        {
            var result = await _contractDraftsService.GetAllContractDraftsAsync();
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager, Staff, Admin, Customer")]
    public async Task<IActionResult> GetContractDraftsById(int id)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("rental/{rentalId}")]
    [Authorize(Roles = "Manager, Staff, Admin, Customer")]
    public async Task<IActionResult> GetContractDraftsByRentalId(int rentalId)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByRentalIdAsync(rentalId);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("staff/{staffId}")]
    [Authorize(Roles = "Manager, Staff, Admin")]
    public async Task<IActionResult> GetContractDraftsByStaffId(int staffId)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByStaffIdAsync(staffId);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("manager/{managerId}")]
    [Authorize(Roles = "Manager, Staff, Admin")]
    public async Task<IActionResult> GetContractDraftsByManagerId(int managerId)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByManagerIdAsync(managerId);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("status/{status}")]
    [Authorize(Roles = "Manager, Staff, Admin")]
    public async Task<IActionResult> GetContractDraftsByStatus(string status)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByStatusAsync(status);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Manager, Staff, Admin")]
    public async Task<IActionResult> CreateContractDrafts([FromBody] CreateContractDraftsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            // Get staff ID from token (the person who creates this contract draft)
            var accountIdClaim = User.FindFirst("accountId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var staffId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "User not authenticated or invalid account ID"
                });
            }
            
            var result = await _contractDraftsService.CreateContractDraftsAsync(request, staffId);
            return CreatedAtAction(nameof(GetContractDraftsById), new { id = result.Id }, new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager, Staff, Admin")]
    public async Task<IActionResult> UpdateContractDrafts(int id, [FromBody] UpdateContractDraftsRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new
            {
                success = false,
                message = "ID mismatch"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var result = await _contractDraftsService.UpdateContractDraftsAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager, Staff, Admin")]
    public async Task<IActionResult> DeleteContractDrafts(int id)
    {
        try
        {
            var result = await _contractDraftsService.DeleteContractDraftsAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Contract draft deleted successfully"
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    // Helper method to get current user ID from token
    private int GetCurrentUserId()
    {
        var accountIdClaim = User.FindFirst("accountId")?.Value;
        if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated or invalid account ID");
        }
        return userId;
    }

    /// <summary>
    /// Manager signs the contract and sends it to customer
    /// </summary>
    [HttpPatch("{id}/manager-sign")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ManagerSignContract(int id, [FromBody] ManagerSignatureRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var managerId = GetCurrentUserId();
            var result = await _contractDraftsService.ManagerSignContractAsync(id, request, managerId);
            
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            // 🔔 Notify Customer: Contract signed by manager
            var rental = await _rentalService.GetRentalAsync(result.RentalId ?? 0);
            if (rental?.AccountId != null)
            {
                await _notificationService.CreateNotificationAsync(
                    rental.AccountId.Value,
                    NotificationType.ContractManagerSigned,
                    "📝 Hợp đồng đã được Manager ký. Vui lòng kiểm tra và ký hợp đồng.",
                    result.RentalId ?? 0,
                    result.Id,
                    isRealTime: true);
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Contract signed by manager and sent to customer"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Customer signs the contract and makes it active
    /// </summary>
    [HttpPatch("{id}/customer-sign")]
    [Authorize]
    public async Task<IActionResult> CustomerSignContract(int id, [FromBody] CustomerSignatureRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var customerId = GetCurrentUserId();
            var result = await _contractDraftsService.CustomerSignContractAsync(id, request, customerId);
            
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            // 🔔 Notify Staff: Contract signed by customer
            var rental = await _rentalService.GetRentalAsync(result.RentalId ?? 0);
            if (rental?.StaffId != null)
            {
                await _notificationService.CreateNotificationAsync(
                    rental.StaffId.Value,
                    NotificationType.ContractCustomerSigned,
                    "🎉 Hợp đồng đã được Customer ký và kích hoạt! Delivery sẽ được tạo.",
                    result.RentalId ?? 0,
                    result.Id,
                    isRealTime: true);
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Contract signed by customer and activated"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Get contracts pending manager signature
    /// </summary>
    [HttpGet("pending-manager-signature")]
    [Authorize]
    public async Task<IActionResult> GetPendingManagerSignatureContracts()
    {
        try
        {
            var managerId = GetCurrentUserId();
            var result = await _contractDraftsService.GetPendingManagerSignatureContractsAsync(managerId);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Get contracts pending customer signature
    /// </summary>
    [HttpGet("pending-customer-signature")]
    [Authorize]
    public async Task<IActionResult> GetPendingCustomerSignatureContracts()
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _contractDraftsService.GetPendingCustomerSignatureContractsAsync(customerId);
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Manager cancels the contract
    /// </summary>
    [HttpPatch("{id}/manager-cancel")]
    [Authorize]
    public async Task<IActionResult> ManagerCancelContract(int id, [FromBody] ManagerCancelRequest request)
    {
        try
        {
            var managerId = GetCurrentUserId();
            var result = await _contractDraftsService.ManagerCancelContractAsync(id, request, managerId);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Contract cancelled by manager"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Customer rejects the contract
    /// </summary>
    [HttpPatch("{id}/customer-reject")]
    [Authorize]
    public async Task<IActionResult> CustomerRejectContract(int id, [FromBody] CustomerRejectRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _contractDraftsService.CustomerRejectContractAsync(id, request, customerId);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Contract rejected by customer"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Customer requests changes to the contract
    /// </summary>
    [HttpPatch("{id}/customer-request-change")]
    [Authorize]
    public async Task<IActionResult> CustomerRequestChange(int id, [FromBody] CustomerRequestChangeRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _contractDraftsService.CustomerRequestChangeAsync(id, request, customerId);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            // 🔔 Notify Staff: Customer requests contract changes
            var rental = await _rentalService.GetRentalAsync(result.RentalId ?? 0);
            if (rental?.StaffId != null)
            {
                await _notificationService.CreateNotificationAsync(
                    rental.StaffId.Value,
                    NotificationType.ContractChangeRequested,
                    $"📋 Customer yêu cầu sửa đổi hợp đồng: {request.Comment}",
                    result.RentalId ?? 0,
                    result.Id,
                    isRealTime: true);
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Change requested by customer"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Get contracts with change requested status (for staff)
    /// </summary>
    [HttpGet("change-requested")]
    [Authorize]
    public async Task<IActionResult> GetChangeRequestedContracts()
    {
        try
        {
            var result = await _contractDraftsService.GetChangeRequestedContractsAsync();
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Staff sends contract to manager for signature
    /// </summary>
    [HttpPatch("{id}/send-to-manager")]
    [Authorize]
    public async Task<IActionResult> SendToManager(int id)
    {
        try
        {
            var staffId = GetCurrentUserId();
            var result = await _contractDraftsService.SendToManagerAsync(id, staffId);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Contract sent to manager for signature"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Staff revises contract after customer requested changes
    /// </summary>
    [HttpPatch("{id}/revise")]
    [Authorize]
    public async Task<IActionResult> ReviseContract(int id, [FromBody] UpdateContractDraftsRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new
            {
                success = false,
                message = "ID mismatch"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var staffId = GetCurrentUserId();
            var result = await _contractDraftsService.ReviseContractAsync(id, request, staffId);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Contract revised successfully"
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Send verification code to customer email for contract signing
    /// </summary>
    [HttpPost("{id}/send-verification-code")]
    [Authorize]
    public async Task<IActionResult> SendVerificationCode(int id)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _contractDraftsService.SendVerificationCodeAsync(id, customerId);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to send verification code"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Verification code sent to your email. Please check your inbox."
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    /// <summary>
    /// Verify the code sent to customer email
    /// </summary>
    [HttpPost("{id}/verify-code")]
    [Authorize]
    public async Task<IActionResult> VerifyCode(int id, [FromBody] VerifyCodeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var customerId = GetCurrentUserId();
            var result = await _contractDraftsService.VerifyCodeAsync(id, request.Code, customerId);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Code verification failed"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Code verified successfully. You can now sign the contract."
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }
}

