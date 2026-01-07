using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/manager/dashboard")]
[ApiController]
// [Authorize(Roles = "Manager,Admin")] // Uncomment for production
public class ManagerDashboardController : ControllerBase
{
    private readonly IManagerDashboardService _dashboardService;

    public ManagerDashboardController(IManagerDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Get overview data for Tab 1: Tổng quan
    /// KPIs, Alerts, Revenue charts, Peak time, Package distribution
    /// </summary>
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        try
        {
            var data = await _dashboardService.GetOverviewAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get dashboard overview", Error = ex.Message });
        }
    }

    /// <summary>
    /// Get robot fleet data for Tab 2: Robots
    /// Fleet overview, status by type, robot list
    /// </summary>
    [HttpGet("robots")]
    public async Task<IActionResult> GetRobots(
        [FromQuery] string? status = null,
        [FromQuery] int? typeId = null)
    {
        try
        {
            var data = await _dashboardService.GetRobotsAsync(status, typeId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get robots data", Error = ex.Message });
        }
    }

    /// <summary>
    /// Get rental data for Tab 3: Đơn thuê
    /// Alerts, statistics, payment status, rental list
    /// </summary>
    [HttpGet("rentals")]
    public async Task<IActionResult> GetRentals(
        [FromQuery] string? status = null,
        [FromQuery] string? package = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var data = await _dashboardService.GetRentalsAsync(status, package, fromDate, toDate, page, pageSize);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get rentals data", Error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer analytics for Tab 4: Khách hàng
    /// Overview, top customers by LTV, segmentation
    /// </summary>
    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers([FromQuery] int topCount = 10)
    {
        try
        {
            var data = await _dashboardService.GetCustomersAsync(topCount);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Failed to get customers data", Error = ex.Message });
        }
    }
}
