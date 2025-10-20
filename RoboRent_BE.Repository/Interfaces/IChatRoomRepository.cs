using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IChatRoomRepository : IGenericRepository<ChatRoom>
{
    Task<ChatRoom?> GetByRentalIdAsync(int rentalId);
    Task<ChatRoom?> GetWithMessagesAsync(int rentalId, int page = 1, int pageSize = 50);
}