using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IRentalService
{
    // Add custom methods here
    public Task<OrderResonse?> CreateRentalAsync(CreateOrderRequest  createOrderRequest);
    public Task<OrderResonse?> UpdateRentalAsync(UpdateOrderRequest updateOrderRequest);
    public Task<OrderResonse?> GetRentalAsync(int id);
    public Task<List<OrderResonse>?> GetAllRentalsAsync();
    public Task<dynamic> DeleteRentalAsync(int id);
}