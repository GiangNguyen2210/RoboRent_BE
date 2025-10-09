using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    private readonly AppDbContext _dbContext;

    public AccountRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }
    
    public async Task<Account?> GetByIdAsync(int accountId)
    {
        return await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId);
    }
    
}