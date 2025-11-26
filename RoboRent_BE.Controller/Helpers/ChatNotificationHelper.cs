using Microsoft.AspNetCore.SignalR;
using RoboRent_BE.Controller.Hubs;
using RoboRent_BE.Model.DTOs.Chat;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Helpers;

public class ChatNotificationHelper
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotificationHelper(IChatService chatService, IHubContext<ChatHub> hubContext)
    {
        _chatService = chatService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Gửi notification vào chat và broadcast qua SignalR
    /// </summary>
    /// <param name="rentalId">ID của rental</param>
    /// <param name="senderId">ID của người gửi (accountId)</param>
    /// <param name="content">Nội dung notification</param>
    /// <param name="priceQuoteId">ID của price quote (optional)</param>
    /// <param name="contractId">ID của contract (optional)</param>
    /// <param name="messageType">Loại message (mặc định: SystemNotification)</param>
    /// <returns>ChatMessageResponse đã được lưu vào DB</returns>
    public async Task<ChatMessageResponse> SendNotificationAsync(
        int rentalId,
        int senderId,
        string content,
        int? priceQuoteId = null,
        int? contractId = null,
        MessageType messageType = MessageType.SystemNotification)
    {
        // Bước 1: Lưu message vào DB qua ChatService
        var message = await _chatService.SendMessageAsync(new SendMessageRequest
        {
            RentalId = rentalId,
            MessageType = messageType,
            Content = content,
            PriceQuoteId = priceQuoteId,
            ContractId = contractId
        }, senderId);

        // Bước 2: Broadcast qua SignalR
        var roomName = $"rental_{rentalId}";
        await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", message);

        return message;
    }
}