using RoboRent_BE.Model.DTOs.Chat;

namespace RoboRent_BE.Service.Interfaces;

public interface IChatService
{
    Task<ChatRoomResponse> GetOrCreateChatRoomAsync(int rentalId, int staffId, int customerId);
    Task<ChatRoomResponse> GetChatRoomByRentalIdAsync(int rentalId);
    Task<ChatMessagesPageResponse> GetChatMessagesAsync(int rentalId, int page = 1, int pageSize = 50);
    Task<ChatMessageResponse> SendMessageAsync(SendMessageRequest request, int senderId);
    Task<ChatMessageResponse> UpdateMessageStatusAsync(int messageId, UpdateMessageStatusRequest request);
    Task<int> GetUnreadCountAsync(int rentalId, int userId);
}