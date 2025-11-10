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
    public Task<List<OrderResponse>?> GetRentalsByCustomerAsync(int accountId);
}