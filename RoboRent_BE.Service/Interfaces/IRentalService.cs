using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IRentalService
{
    // Add custom methods here
    public Task<OrderResponse?> CreateRentalAsync(CreateOrderRequest  createOrderRequest);
    public Task<OrderResponse?> UpdateRentalAsync(UpdateOrderRequest updateOrderRequest);
    public Task<OrderResponse?> GetRentalAsync(int id);
    public Task<List<OrderResponse>?> GetAllRentalsAsync();
    public Task<dynamic> DeleteRentalAsync(int id);
    public Task<PageListResponse<OrderResponse>> GetRentalByCustomerIdAsync(int customerId, int page, int pageSize, string? search);
    public Task<OrderResponse?> CustomerSendRentalAsync(int rentalId);
    public Task<List<OrderResponse>?> GetRentalsByCustomerAsync(int accountId);
    public Task<List<OrderResponse>> GetAllPendingRentalsAsync();
    public Task<OrderResponse?> ReceiveRequestAsync(int  rentalId, int staffId);
    public Task<List<OrderResponse>> GetAllReceivedRentalsByStaffId(int staffId);
    public Task<OrderResponse?> StaffUpdateRentalInfoAsync(int rentalId, StaffUpdateRequest  staffUpdateRequest);
    public Task<OrderResponse?> CustomerCancelRentalAsync(int rentalId);
    public Task<OrderResponse?> CustomerDeleteRentalAsync(int rentalId);
    Task<RentalCompletionResponse?> CompleteRentalAsync(int rentalId);

}