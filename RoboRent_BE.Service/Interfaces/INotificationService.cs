using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Service.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Create a notification for the notification bell + optional SignalR push
    /// </summary>
    Task<Notification> CreateNotificationAsync(
        int recipientId,
        NotificationType type,
        string content,
        int? rentalId = null,
        int? relatedEntityId = null,
        bool isRealTime = true);

    /// <summary>
    /// Create notifications for multiple recipients
    /// </summary>
    Task CreateNotificationsAsync(
        IEnumerable<int> recipientIds,
        NotificationType type,
        string content,
        int? rentalId = null,
        int? relatedEntityId = null,
        bool isRealTime = true);
}
