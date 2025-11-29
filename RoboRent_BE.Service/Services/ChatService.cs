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
    private readonly IRentalRepository _rentalRepo;

    public ChatService(
        IChatRoomRepository chatRoomRepo,
        IChatMessageRepository chatMessageRepo,
        IGenericRepository<Account> accountRepo,
        IRentalRepository rentalRepo)
    {
        _chatRoomRepo = chatRoomRepo;
        _chatMessageRepo = chatMessageRepo;
        _accountRepo = accountRepo;
        _rentalRepo = rentalRepo;
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

        var rental = await _rentalRepo.GetAsync(r => r.Id == request.RentalId);
        
        if (room == null)
        {
            throw new Exception($"Chat room not found for rental {request.RentalId}");
        }

        var sender = await _accountRepo.GetAsync(a => a.Id == senderId);
        if (sender == null)
        {
            throw new Exception($"Sender account not found");
        }

        if (rental.Status != "PendingDemo" && request.MessageType == MessageType.Demo)
        {
            rental.Status = "PendingDemo";
            await _rentalRepo.UpdateAsync(rental);
        }
        
        var message = new ChatMessage
        {
            ChatRoomId = room.Id,
            SenderId = senderId,
            MessageType = request.MessageType,
            Content = request.Content,
            MediaUrls = request.VideoUrls != null ? JsonSerializer.Serialize(request.VideoUrls) : null,
            PriceQuoteId = request.PriceQuoteId,
            ContractId = request.ContractId,
            Status = request.MessageType == MessageType.Demo ? "Pending" : null,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _chatMessageRepo.AddAsync(message);
        var chatRoom = await _chatRoomRepo.GetByRentalIdAsync(request.RentalId); 
        if (chatRoom != null)
        {
            chatRoom.UpdatedAt = DateTime.UtcNow;
            await _chatRoomRepo.UpdateAsync(chatRoom);
        }
        
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

        var rental = await _rentalRepo.GetAsync(r => r.Id == message.ChatRoom.RentalId);

        if (request.Status == "Accepted")
        {
            rental.Status = "AcceptedDemo";
        } else if (request.Status == "Rejected")
        {
            rental.Status = "DeniedDemo";
        }
        
        await _rentalRepo.UpdateAsync(rental);
        
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
    
    public async Task<ChatRoomListResponse> GetChatRoomsByStaffIdAsync(int staffId, int page = 1, int pageSize = 50)
    {
        var roomsPage = await _chatRoomRepo.GetRoomsByStaffIdAsync(staffId, page, pageSize);
        
        var roomDtos = new List<ChatRoomListItemDto>();
        
        foreach (var room in roomsPage.Items)
        {
            var lastMessage = room.Messages?.FirstOrDefault();
            var unreadCount = await _chatMessageRepo.CountUnreadMessagesAsync(room.Id, staffId);
            
            roomDtos.Add(new ChatRoomListItemDto
            {
                Id = room.Id,
                RentalId = room.RentalId,
                CustomerName = room.Customer?.FullName ?? "Unknown Customer",
                PackageName = room.Rental?.ActivityType?.Name ?? "Unknown Package",  // ✅ FIX
                EventDate = room.Rental?.EventDate?.ToString("MMM dd, yyyy") ?? "TBD", // ✅ FIX
                Status = room.Rental?.Status ?? "Unknown",
                RentalStatus = room.Rental?.Status ?? "Unknown",  
                LastMessage = lastMessage?.Content ?? "No messages yet",
                LastMessageTime = lastMessage?.CreatedAt,
                UnreadCount = unreadCount,
                CreatedAt = room.CreatedAt
            });
        }
        
        return new ChatRoomListResponse
        {
            Rooms = roomDtos,
            Page = roomsPage.Page,
            PageSize = roomsPage.PageSize,
            TotalCount = roomsPage.TotalCount,
            HasNextPage = roomsPage.HasNextPage,
            HasPreviousPage = roomsPage.HasPreviousPage
        };
    }

    public async Task<ChatRoomListResponse> GetChatRoomsByCustomerIdAsync(int customerId, int page = 1, int pageSize = 50)
    {
        var roomsPage = await _chatRoomRepo.GetRoomsByCustomerIdAsync(customerId, page, pageSize);
        
        var roomDtos = new List<ChatRoomListItemDto>();
        
        foreach (var room in roomsPage.Items)
        {
            var lastMessage = room.Messages?.FirstOrDefault();
            var unreadCount = await _chatMessageRepo.CountUnreadMessagesAsync(room.Id, customerId);
            
            roomDtos.Add(new ChatRoomListItemDto
            {
                Id = room.Id,
                RentalId = room.RentalId,
                StaffName = room.Staff?.FullName ?? "Staff",
                PackageName = room.Rental?.ActivityType?.Name ?? "Unknown Package",  // ✅ FIX
                EventDate = room.Rental?.EventDate?.ToString("MMM dd, yyyy") ?? "TBD", // ✅ FIX
                Status = room.Rental?.Status ?? "Unknown",
                RentalStatus = room.Rental?.Status ?? "Unknown", 
                LastMessage = lastMessage?.Content ?? "No messages yet",
                LastMessageTime = lastMessage?.CreatedAt,
                UnreadCount = unreadCount,
                CreatedAt = room.CreatedAt
            });
        }
        
        return new ChatRoomListResponse
        {
            Rooms = roomDtos,
            Page = roomsPage.Page,
            PageSize = roomsPage.PageSize,
            TotalCount = roomsPage.TotalCount,
            HasNextPage = roomsPage.HasNextPage,
            HasPreviousPage = roomsPage.HasPreviousPage
        };
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
            RentalId = message.ChatRoom?.RentalId ?? 0,
            SenderId = message.SenderId,
            SenderName = message.Sender?.FullName ?? "Unknown",
            SenderRole = DetermineSenderRole(message),
            MessageType = message.MessageType,
            Content = message.Content,
            VideoUrls = videoUrls,
            PriceQuoteId = message.PriceQuoteId,
            ContractId = message.ContractId,
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