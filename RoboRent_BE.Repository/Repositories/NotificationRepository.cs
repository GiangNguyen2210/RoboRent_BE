using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<List<Notification>> GetByRecipientIdAsync(int recipientId, int page = 1, int pageSize = 20)
    {
        return await _context.Notifications
            .Where(n => n.RecipientId == recipientId && n.DeletedAt == null)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(n => n.Rental)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int recipientId)
    {
        return await _context.Notifications
            .CountAsync(n => n.RecipientId == recipientId && !n.IsRead && n.DeletedAt == null);
    }

    public async Task<int> GetTotalCountAsync(int recipientId)
    {
        return await _context.Notifications
            .CountAsync(n => n.RecipientId == recipientId && n.DeletedAt == null);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null && notification.DeletedAt == null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int recipientId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.RecipientId == recipientId && !n.IsRead && n.DeletedAt == null)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null && notification.DeletedAt == null)
        {
            notification.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task SoftDeleteAllAsync(int recipientId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.RecipientId == recipientId && n.DeletedAt == null)
            .ToListAsync();

        foreach (var n in notifications)
            n.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}

