using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class NotificationService : INotificationService
{
    private readonly ISignalRBroadcaster _broadcaster;
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        ISignalRBroadcaster broadcaster,
        INotificationRepository notificationRepository)
    {
        _broadcaster = broadcaster;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Create a notification for the notification bell + optional SignalR push
    /// </summary>
    public async Task<Notification> CreateNotificationAsync(
        int recipientId,
        NotificationType type,
        string content,
        int? rentalId = null,
        int? relatedEntityId = null,
        bool isRealTime = true)
    {
        var notification = new Notification
        {
            RecipientId = recipientId,
            Type = type,
            Content = content,
            RentalId = rentalId,
            RelatedEntityId = relatedEntityId,
            IsRealTime = isRealTime,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _notificationRepository.CreateAsync(notification);

        // If real-time, also push via SignalR
        if (isRealTime)
        {
            await _broadcaster.BroadcastToUserAsync(recipientId.ToString(), "NewNotification", new
            {
                created.Id,
                created.Type,
                TypeName = created.Type.ToString(),
                created.Content,
                created.RentalId,
                created.RelatedEntityId,
                created.CreatedAt
            });
        }

        return created;
    }

    /// <summary>
    /// Create notifications for multiple recipients
    /// </summary>
    public async Task CreateNotificationsAsync(
        IEnumerable<int> recipientIds,
        NotificationType type,
        string content,
        int? rentalId = null,
        int? relatedEntityId = null,
        bool isRealTime = true)
    {
        foreach (var recipientId in recipientIds)
        {
            await CreateNotificationAsync(recipientId, type, content, rentalId, relatedEntityId, isRealTime);
        }
    }
}
