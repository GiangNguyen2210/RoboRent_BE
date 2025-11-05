using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class RentalDetailRepository : GenericRepository<RentalDetail>, IRentalDetailRepository
{
    private readonly AppDbContext _dbContext;
    public RentalDetailRepository(AppDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<RentalDetail>> GetRentalDetailsByRentalIdAsync(int rentalId)
    {
        return await _dbContext.RentalDetails
            .Where(rd => rd.RentalId == rentalId && rd.IsDeleted == false)
            .Include(rd => rd.RoboType)
            .Include(rd => rd.Rental)
            .ToListAsync();
    }

    public async Task<IEnumerable<RentalDetail>> GetRentalDetailsByRoboTypeIdAsync(int roboTypeId)
    {
        return await _dbContext.RentalDetails
            .Where(rd => rd.RoboTypeId == roboTypeId && rd.IsDeleted == false)
            .Include(rd => rd.RoboType)
            .Include(rd => rd.Rental)
            .ToListAsync();
    }

    public async Task<IEnumerable<RentalDetail>> GetAllWithIncludesAsync()
    {
        return await _dbContext.RentalDetails
            .Where(rd => rd.IsDeleted == false)
            .Include(rd => rd.RoboType)
            .Include(rd => rd.Rental)
            .ToListAsync();
    }
}