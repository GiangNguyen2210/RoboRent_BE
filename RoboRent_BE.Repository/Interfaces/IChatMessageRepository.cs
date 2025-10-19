using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IChatMessageRepository : IGenericRepository<ChatMessage>
{
    Task<PageListResponse<ChatMessage>> GetMessagesByRoomIdAsync(int chatRoomId, int page = 1, int pageSize = 50);
    Task<ChatMessage?> GetByIdWithDetailsAsync(int messageId);
    Task<int> CountUnreadMessagesAsync(int chatRoomId, int userId);
    Task MarkAsReadAsync(int messageId);
}