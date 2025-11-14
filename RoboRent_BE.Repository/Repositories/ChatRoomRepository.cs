using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ChatRoomRepository : GenericRepository<ChatRoom>, IChatRoomRepository
{
    private readonly AppDbContext _dbContext;

    public ChatRoomRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<ChatRoom?> GetByRentalIdAsync(int rentalId)
    {
        return await DbSet
            .Include(cr => cr.Staff)
            .Include(cr => cr.Customer)
            .Include(cr => cr.Rental)
            .FirstOrDefaultAsync(cr => cr.RentalId == rentalId);
    }

    public async Task<ChatRoom?> GetWithMessagesAsync(int rentalId, int page = 1, int pageSize = 50)
    {
        return await DbSet
            .Include(cr => cr.Staff)
            .Include(cr => cr.Customer)
            .Include(cr => cr.Messages.OrderByDescending(m => m.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize))
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(cr => cr.RentalId == rentalId);
    }
    public async Task<PageListResponse<ChatRoom>> GetRoomsByStaffIdAsync(int staffId, int page = 1, int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;

        var query = _dbContext.ChatRooms
            .Include(cr => cr.Customer)
            .Include(cr => cr.Rental)
                .ThenInclude(r => r.ActivityType)           // ✅ FIX
            .Include(cr => cr.Rental)
                .ThenInclude(r => r.EventActivity)          // ✅ FIX
            .Include(cr => cr.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Sender)
            .Where(cr => cr.StaffId == staffId)
            .OrderByDescending(cr => cr.UpdatedAt ?? cr.CreatedAt);

        var totalCount = await query.CountAsync();

        var rooms = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageListResponse<ChatRoom>
        {
            Items = rooms,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = (page * pageSize) < totalCount,
            HasPreviousPage = page > 1
        };
    }

    public async Task<PageListResponse<ChatRoom>> GetRoomsByCustomerIdAsync(int customerId, int page = 1, int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;

        var query = _dbContext.ChatRooms
            .Include(cr => cr.Staff)
            .Include(cr => cr.Rental)
                .ThenInclude(r => r.ActivityType)           // ✅ FIX
            .Include(cr => cr.Rental)
                .ThenInclude(r => r.EventActivity)          // ✅ FIX
            .Include(cr => cr.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Sender)
            .Where(cr => cr.CustomerId == customerId)
            .OrderByDescending(cr => cr.UpdatedAt ?? cr.CreatedAt);

        var totalCount = await query.CountAsync();

        var rooms = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageListResponse<ChatRoom>
        {
            Items = rooms,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = (page * pageSize) < totalCount,
            HasPreviousPage = page > 1
        };
    }
}