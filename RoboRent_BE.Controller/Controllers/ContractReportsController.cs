using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.ContractReports;
using RoboRent_BE.Service.Interfaces;
using System.Security.Claims;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ContractReportsController : ControllerBase
{
    private readonly IContractReportsService _contractReportsService;

    public ContractReportsController(IContractReportsService contractReportsService)
    {
        _contractReportsService = contractReportsService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("accountId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return int.Parse(userIdClaim);
    }

    private string GetCurrentUserRole()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return role ?? "Customer";
    }

    /// <summary>
    /// Get all contract reports
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Manager,Staff,Admin")]
    public async Task<IActionResult> GetAllContractReports()
    {
        try
        {
            var result = await _contractReportsService.GetAllContractReportsAsync();
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
    /// Get contract reports for the authenticated user (Customer, Staff, Manager)
    /// </summary>
    [HttpGet("my-reports")]
    [Authorize(Roles = "Customer,Staff,Manager")]
    public async Task<IActionResult> GetMyContractReports()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _contractReportsService.GetContractReportsByUserIdAsync(userId);
            return Ok(new
            {
                success = true,
                data = result
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
    /// Get contract report by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Staff,Admin,Customer")]
    public async Task<IActionResult> GetContractReportById(int id)
    {
        try
        {
            var result = await _contractReportsService.GetContractReportByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract report not found"
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

    /// <summary>
    /// Get contract reports with status Pending (for manager review)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Manager,Staff,Admin")]
    public async Task<IActionResult> GetPendingContractReports()
    {
        try
        {
            var result = await _contractReportsService.GetPendingContractReportsAsync();
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
    /// Create a new contract report
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Manager,Staff,Admin")]
    public async Task<IActionResult> CreateContractReport([FromBody] CreateContractReportRequest request)
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
            var reporterId = GetCurrentUserId();
            var reportRole = GetCurrentUserRole();

            var result = await _contractReportsService.CreateContractReportAsync(request, reporterId, reportRole);
            
            return Ok(new
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

    /// <summary>
    /// Manager resolves a contract report (creates payment and updates status)
    /// </summary>
    [HttpPatch("{id}/resolve")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ResolveContractReport(int id, [FromBody] ResolveContractReportRequest request)
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
            var result = await _contractReportsService.ResolveContractReportAsync(id, request, managerId);
            
            return Ok(new
            {
                success = true,
                message = "Contract report resolved successfully",
                data = result,
                paymentLink = result.PaymentLink
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

    /// <summary>
    /// Manager rejects a contract report
    /// </summary>
    [HttpPatch("{id}/reject")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> RejectContractReport(int id, [FromBody] RejectContractReportRequest request)
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
            var result = await _contractReportsService.RejectContractReportAsync(id, request, managerId);
            
            return Ok(new
            {
                success = true,
                message = "Contract report rejected successfully",
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

    /// <summary>
    /// Delete a contract report by ID
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Staff,Admin")]
    public async Task<IActionResult> DeleteContractReport(int id)
    {
        try
        {
            var result = await _contractReportsService.DeleteContractReportAsync(id);
            
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract report not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Contract report deleted successfully"
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

