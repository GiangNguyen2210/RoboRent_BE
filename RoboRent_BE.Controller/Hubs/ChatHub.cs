using Microsoft.AspNetCore.SignalR;
using RoboRent_BE.Model.DTOs.Chat;

namespace RoboRent_BE.Controller.Hubs;

public class ChatHub : Hub
{
    // Khi user connect, join vào room của rental
    public async Task JoinRentalChat(int rentalId)
    {
        var roomName = $"rental_{rentalId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        
        // Notify others in room
        await Clients.OthersInGroup(roomName).SendAsync("UserJoined", Context.ConnectionId);
    }

    // Leave room
    public async Task LeaveRentalChat(int rentalId)
    {
        var roomName = $"rental_{rentalId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        
        await Clients.OthersInGroup(roomName).SendAsync("UserLeft", Context.ConnectionId);
    }

    // Server sẽ gọi method này để broadcast message tới room
    public async Task SendMessageToRoom(int rentalId, ChatMessageResponse message)
    {
        var roomName = $"rental_{rentalId}";
        await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
    }

    // Notify typing indicator
    public async Task UserTyping(int rentalId, string userName)
    {
        var roomName = $"rental_{rentalId}";
        await Clients.OthersInGroup(roomName).SendAsync("UserIsTyping", userName);
    }

    // Notify demo status changed
    public async Task NotifyDemoStatusChanged(int rentalId, int messageId, string status)
    {
        var roomName = $"rental_{rentalId}";
        await Clients.Group(roomName).SendAsync("DemoStatusChanged", messageId, status);
    }

    // Override disconnect
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}