using RoboRent_BE.Model.DTOs.Dashboard;

namespace RoboRent_BE.Service.Interfaces;

public interface IManagerDashboardService
{
    /// <summary>
    /// Get overview data for Tab 1: Tổng quan
    /// </summary>
    Task<DashboardOverviewResponse> GetOverviewAsync();

    /// <summary>
    /// Get robot fleet data for Tab 2: Robots
    /// </summary>
    Task<DashboardRobotsResponse> GetRobotsAsync(string? status = null, int? typeId = null);

    /// <summary>
    /// Get rental data for Tab 3: Đơn thuê
    /// </summary>
    Task<DashboardRentalsResponse> GetRentalsAsync(
        string? status = null,
        string? package = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Get customer analytics for Tab 4: Khách hàng
    /// </summary>
    Task<DashboardCustomersResponse> GetCustomersAsync(int topCount = 10);
}
