using System.Text.Json;
using RoboRent_BE.Model.DTOs.Chat;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ChatService : IChatService
{
    private readonly IChatRoomRepository _chatRoomRepo;
    private readonly IChatMessageRepository _chatMessageRepo;
    private readonly IGenericRepository<Account> _accountRepo;

    public ChatService(
        IChatRoomRepository chatRoomRepo,
        IChatMessageRepository chatMessageRepo,
        IGenericRepository<Account> accountRepo)
    {
        _chatRoomRepo = chatRoomRepo;
        _chatMessageRepo = chatMessageRepo;
        _accountRepo = accountRepo;
    }

    public async Task<ChatRoomResponse> GetOrCreateChatRoomAsync(int rentalId, int staffId, int customerId)
    {
        var existingRoom = await _chatRoomRepo.GetByRentalIdAsync(rentalId);
        
        if (existingRoom != null)
        {
            return MapToChatRoomResponse(existingRoom);
        }

        var newRoom = new ChatRoom
        {
            RentalId = rentalId,
            StaffId = staffId,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        };

        await _chatRoomRepo.AddAsync(newRoom);
        
        var createdRoom = await _chatRoomRepo.GetByRentalIdAsync(rentalId);
        return MapToChatRoomResponse(createdRoom!);
    }

    public async Task<ChatRoomResponse> GetChatRoomByRentalIdAsync(int rentalId)
    {
        var room = await _chatRoomRepo.GetByRentalIdAsync(rentalId);
        
        if (room == null)
        {
            throw new Exception($"Chat room not found for rental {rentalId}");
        }

        return MapToChatRoomResponse(room);
    }

    public async Task<ChatMessagesPageResponse> GetChatMessagesAsync(int rentalId, int page = 1, int pageSize = 50)
    {
        var room = await _chatRoomRepo.GetByRentalIdAsync(rentalId);
        
        if (room == null)
        {
            throw new Exception($"Chat room not found for rental {rentalId}");
        }

        var messagesPage = await _chatMessageRepo.GetMessagesByRoomIdAsync(room.Id, page, pageSize);

        return new ChatMessagesPageResponse
        {
            Messages = messagesPage.Items.Select(MapToChatMessageResponse).ToList(),
            Page = messagesPage.Page,
            PageSize = messagesPage.PageSize,
            TotalCount = messagesPage.TotalCount,
            HasNextPage = messagesPage.HasNextPage,
            HasPreviousPage = messagesPage.HasPreviousPage
        };
    }

    public async Task<ChatMessageResponse> SendMessageAsync(SendMessageRequest request, int senderId)
    {
        var room = await _chatRoomRepo.GetByRentalIdAsync(request.RentalId);
        
        if (room == null)
        {
            throw new Exception($"Chat room not found for rental {request.RentalId}");
        }

        var sender = await _accountRepo.GetAsync(a => a.Id == senderId);
        if (sender == null)
        {
            throw new Exception($"Sender account not found");
        }

        var message = new ChatMessage
        {
            ChatRoomId = room.Id,
            SenderId = senderId,
            MessageType = request.MessageType,
            Content = request.Content,
            MediaUrls = request.VideoUrls != null ? JsonSerializer.Serialize(request.VideoUrls) : null,
            RelatedEntityId = request.RelatedEntityId,
            Status = request.MessageType == MessageType.Demo ? "Pending" : null,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _chatMessageRepo.AddAsync(message);
        
        var createdMessage = await _chatMessageRepo.GetByIdWithDetailsAsync(message.Id);
        
        // ✅ Không lo SignalR nữa - Controller sẽ lo
        return MapToChatMessageResponse(createdMessage!);
    }

    public async Task<ChatMessageResponse> UpdateMessageStatusAsync(int messageId, UpdateMessageStatusRequest request)
    {
        var message = await _chatMessageRepo.GetByIdWithDetailsAsync(messageId);
        
        if (message == null)
        {
            throw new Exception($"Message not found");
        }

        if (message.MessageType != MessageType.Demo)
        {
            throw new Exception("Only demo messages can have status updated");
        }

        message.Status = request.Status;
        await _chatMessageRepo.UpdateAsync(message);

        // ✅ Không lo SignalR - Controller sẽ broadcast
        return MapToChatMessageResponse(message);
    }

    public async Task<int> GetUnreadCountAsync(int rentalId, int userId)
    {
        var room = await _chatRoomRepo.GetByRentalIdAsync(rentalId);
        
        if (room == null)
        {
            return 0;
        }

        return await _chatMessageRepo.CountUnreadMessagesAsync(room.Id, userId);
    }

    // Helper methods
    private ChatRoomResponse MapToChatRoomResponse(ChatRoom room)
    {
        return new ChatRoomResponse
        {
            Id = room.Id,
            RentalId = room.RentalId,
            Staff = new StaffInfoDto
            {
                Id = room.Staff.Id,
                FullName = room.Staff.FullName ?? "Unknown",
                PhoneNumber = room.Staff.PhoneNumber
            },
            Customer = new CustomerInfoDto
            {
                Id = room.Customer.Id,
                FullName = room.Customer.FullName ?? "Unknown",
                PhoneNumber = room.Customer.PhoneNumber
            },
            CreatedAt = room.CreatedAt,
            Messages = room.Messages?.Select(MapToChatMessageResponse).ToList() ?? new List<ChatMessageResponse>()
        };
    }

    private ChatMessageResponse MapToChatMessageResponse(ChatMessage message)
    {
        List<string>? videoUrls = null;
        if (!string.IsNullOrEmpty(message.MediaUrls))
        {
            try
            {
                videoUrls = JsonSerializer.Deserialize<List<string>>(message.MediaUrls);
            }
            catch { }
        }

        return new ChatMessageResponse
        {
            Id = message.Id,
            ChatRoomId = message.ChatRoomId,
            SenderId = message.SenderId,
            SenderName = message.Sender?.FullName ?? "Unknown",
            SenderRole = DetermineSenderRole(message),
            MessageType = message.MessageType,
            Content = message.Content,
            VideoUrls = videoUrls,
            RelatedEntityId = message.RelatedEntityId,
            Status = message.Status,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt
        };
    }

    private string DetermineSenderRole(ChatMessage message)
    {
        if (message.ChatRoom?.StaffId == message.SenderId)
            return "Staff";
        if (message.ChatRoom?.CustomerId == message.SenderId)
            return "Customer";
        return "System";
    }
}