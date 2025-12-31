namespace RoboRent_BE.Service.Interfaces;

/// <summary>
/// Abstraction for SignalR broadcasting (implemented in Controller layer)
/// </summary>
public interface ISignalRBroadcaster
{
    /// <summary>
    /// Broadcast message to a SignalR group
    /// </summary>
    Task BroadcastToGroupAsync<T>(string groupName, string methodName, T payload);

    /// <summary>
    /// Broadcast message to a specific user
    /// </summary>
    Task BroadcastToUserAsync<T>(string userId, string methodName, T payload);

    /// <summary>
    /// Broadcast message to multiple users
    /// </summary>
    Task BroadcastToUsersAsync<T>(IEnumerable<string> userIds, string methodName, T payload);
}
