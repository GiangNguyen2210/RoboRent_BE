using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
{
    private readonly AppDbContext _dbContext;

    public ChatMessageRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<PageListResponse<ChatMessage>> GetMessagesByRoomIdAsync(int chatRoomId, int page = 1, int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;

        var query = DbSet
            .Include(m => m.Sender)
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderBy(m => m.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageListResponse<ChatMessage>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = (page * pageSize) < totalCount,
            HasPreviousPage = page > 1
        };
    }

    public async Task<ChatMessage?> GetByIdWithDetailsAsync(int messageId)
    {
        return await DbSet
            .Include(m => m.Sender)
            .Include(m => m.ChatRoom)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<int> CountUnreadMessagesAsync(int chatRoomId, int userId)
    {
        return await DbSet
            .Where(m => m.ChatRoomId == chatRoomId && m.SenderId != userId && !m.IsRead)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(int messageId)
    {
        var message = await DbSet.FindAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}