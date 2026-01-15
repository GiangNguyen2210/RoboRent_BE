using Microsoft.AspNetCore.SignalR;
using RoboRent_BE.Controller.Hubs;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Infrastructure;

/// <summary>
/// Implementation of ISignalRBroadcaster using ChatHub
/// </summary>
public class SignalRBroadcaster : ISignalRBroadcaster
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRBroadcaster(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastToGroupAsync<T>(string groupName, string methodName, T payload)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(methodName, payload);
    }

    public async Task BroadcastToUserAsync<T>(string userId, string methodName, T payload)
    {
        await _hubContext.Clients.User(userId).SendAsync(methodName, payload);
    }

    public async Task BroadcastToUsersAsync<T>(IEnumerable<string> userIds, string methodName, T payload)
    {
        await _hubContext.Clients.Users(userIds.ToList()).SendAsync(methodName, payload);
    }
}
