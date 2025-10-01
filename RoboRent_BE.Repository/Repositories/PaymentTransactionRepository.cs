using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interface;

namespace RoboRent_BE.Repository.Repositories;

public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
{
    private readonly AppDbContext _dbContext;
    
    public PaymentTransactionRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }
    public async Task<IEnumerable<PaymentTransaction>> GetByAccountIdAsync(int accountId)
    {
        return await _dbContext.PaymentTransactions
            .Where(pt => pt.AccountId == accountId)
            .Include(pt => pt.Account) // Include Account nếu cần dữ liệu liên kết
            .ToListAsync();
    }

    public async Task<long> GetLastOrderCodeAsync()
    {
        var last = await _dbContext.PaymentTransactions
            .OrderByDescending(pt => pt.OrderCode)
            .Select(pt => pt.OrderCode)
            .FirstOrDefaultAsync();
        return last;
    }

    public async Task<PaymentTransaction?> GetByOrderCodeAsync(long orderCode)
    {
        return await _dbContext.PaymentTransactions
            .Include(pt => pt.Account) // Include Account cho webhook nếu cần
            .FirstOrDefaultAsync(pt => pt.OrderCode == orderCode);
    }
}