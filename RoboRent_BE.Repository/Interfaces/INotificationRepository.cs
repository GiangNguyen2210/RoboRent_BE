using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Service.Interfaces;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification);
    Task<List<Notification>> GetByRecipientIdAsync(int recipientId, int page = 1, int pageSize = 20);
    Task<int> GetUnreadCountAsync(int recipientId);
    Task<int> GetTotalCountAsync(int recipientId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int recipientId);
    Task SoftDeleteAsync(int notificationId);
    Task SoftDeleteAllAsync(int recipientId);
}
