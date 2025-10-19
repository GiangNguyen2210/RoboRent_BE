using Microsoft.EntityFrameworkCore;
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
}