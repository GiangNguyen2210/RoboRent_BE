using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ContractDraftsRepository : GenericRepository<ContractDrafts>, IContractDraftsRepository
{
    private readonly AppDbContext _dbContext;
    public ContractDraftsRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<ContractDrafts>> GetContractDraftsByRentalIdAsync(int rentalId)
    {
        return await _dbContext.ContractDrafts
            .Where(cd => cd.RentalId == rentalId)
            .Include(cd => cd.ContractTemplate)
            .Include(cd => cd.Rental)
            .Include(cd => cd.Staff)
            .Include(cd => cd.Manager)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractDrafts>> GetContractDraftsByStaffIdAsync(int staffId)
    {
        return await _dbContext.ContractDrafts
            .Where(cd => cd.StaffId == staffId)
            .Include(cd => cd.ContractTemplate)
            .Include(cd => cd.Rental)
            .Include(cd => cd.Staff)
            .Include(cd => cd.Manager)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractDrafts>> GetContractDraftsByManagerIdAsync(int managerId)
    {
        return await _dbContext.ContractDrafts
            .Where(cd => cd.ManagerId == managerId)
            .Include(cd => cd.ContractTemplate)
            .Include(cd => cd.Rental)
            .Include(cd => cd.Staff)
            .Include(cd => cd.Manager)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractDrafts>> GetContractDraftsByStatusAsync(string status)
    {
        return await _dbContext.ContractDrafts
            .Where(cd => cd.Status == status)
            .Include(cd => cd.ContractTemplate)
            .Include(cd => cd.Rental)
            .Include(cd => cd.Staff)
            .Include(cd => cd.Manager)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractDrafts>> GetAllWithIncludesAsync()
    {
        return await _dbContext.ContractDrafts
            .Include(cd => cd.ContractTemplate)
            .Include(cd => cd.Rental)
            .Include(cd => cd.Staff)
            .Include(cd => cd.Manager)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContractDrafts>> GetContractDraftsByCustomerIdAsync(int customerId)
    {
        return await _dbContext.ContractDrafts
            .Where(cd => cd.Rental != null && cd.Rental.AccountId == customerId)
            .Include(cd => cd.ContractTemplate)
            .Include(cd => cd.Rental)
            .ThenInclude(r => r.Account)
            .Include(cd => cd.Staff)
            .Include(cd => cd.Manager)
            .ToListAsync();
    }
}
