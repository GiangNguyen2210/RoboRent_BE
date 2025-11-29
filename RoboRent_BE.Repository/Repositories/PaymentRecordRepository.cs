using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class PaymentRecordRepository : GenericRepository<PaymentRecord>, IPaymentRecordRepository
{
    private readonly AppDbContext _dbContext;
    
    public PaymentRecordRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<PaymentRecord?> GetByOrderCodeAsync(long orderCode)
    {
        return await _dbContext.PaymentRecords
            .Include(pr => pr.Rental)
            .ThenInclude(r => r.Account)
            .Include(pr => pr.PriceQuote)
            .FirstOrDefaultAsync(pr => pr.OrderCode == orderCode);
    }

    public async Task<List<PaymentRecord>> GetByRentalIdAsync(int rentalId)
    {
        return await _dbContext.PaymentRecords
            .Include(pr => pr.Rental)
            .Include(pr => pr.PriceQuote)
            .Where(pr => pr.RentalId == rentalId)
            .OrderBy(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<PaymentRecord?> GetByRentalIdAndTypeAsync(int rentalId, string paymentType)
    {
        return await _dbContext.PaymentRecords
            .Include(pr => pr.Rental)
            .Include(pr => pr.PriceQuote)
            .FirstOrDefaultAsync(pr => pr.RentalId == rentalId && pr.PaymentType == paymentType);
    }

    public async Task<long> GetLastOrderCodeAsync()
    {
        var last = await _dbContext.PaymentRecords
            .OrderByDescending(pr => pr.OrderCode)
            .Select(pr => pr.OrderCode)
            .FirstOrDefaultAsync();
        return last;
    }
}