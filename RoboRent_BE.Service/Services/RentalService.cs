using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class RentalService : IRentalService
{
    private readonly IMapper _mapper;
    private readonly IRentalRepository _rentalRepository;
    public RentalService(IMapper mapper,  IRentalRepository rentalRepository)
    {
        _mapper = mapper;
        _rentalRepository = rentalRepository;
    }

    public async Task<OrderResponse?> CreateRentalAsync(CreateOrderRequest createOrderRequest)
    {
        // Validate foreign keys first
        var accountExists = await _rentalRepository.GetDbContext().Accounts.AnyAsync(a => a.Id == createOrderRequest.AccountId);
        var eventExists = await _rentalRepository.GetDbContext().EventActivities.AnyAsync(e => e.Id == createOrderRequest.EventActivityId);
        var packageExists = await _rentalRepository.GetDbContext().ActivityTypes.AnyAsync(p => p.Id == createOrderRequest.ActivityTypeId);

        if (!accountExists || !eventExists || !packageExists)
            throw new ArgumentException("Invalid foreign key reference. Please check Account, Event, or RentalPackage ID.");

        Rental rental = _mapper.Map<Rental>(createOrderRequest);
        
        rental.CreatedDate = DateTime.UtcNow;
        rental.UpdatedDate = DateTime.UtcNow;

        await _rentalRepository.AddAsync(rental);
        
        return rental == null ? null : _mapper.Map<OrderResponse>(rental);
    }

    public async Task<OrderResponse?> UpdateRentalAsync(UpdateOrderRequest updateOrderRequest)
    {
        // Validate foreign keys first
        var accountExists = await _rentalRepository.GetDbContext().Accounts.AnyAsync(a => a.Id == updateOrderRequest.AccountId);
        var eventExists = await _rentalRepository.GetDbContext().EventActivities.AnyAsync(e => e.Id == updateOrderRequest.EventActivityId);
        var packageExists = await _rentalRepository.GetDbContext().ActivityTypes.AnyAsync(p => p.Id == updateOrderRequest.ActivityTypeId);

        if (!accountExists || !eventExists || !packageExists)
            throw new ArgumentException("Invalid foreign key reference. Please check Account, Event, or RentalPackage ID.");

        Rental? rental = await _rentalRepository.GetDbContext().Rentals.FirstOrDefaultAsync(r => r.Id == updateOrderRequest.Id);

        if (rental == null) return null;

        _mapper.Map(updateOrderRequest, rental);
        
        rental.UpdatedDate = DateTime.UtcNow;
        
        await _rentalRepository.UpdateAsync(rental);
        
        return _mapper.Map<OrderResponse>(rental);
    }

    public async Task<OrderResponse?> GetRentalAsync(int id)
    {
        Expression<Func<Rental, bool>> filter = r =>
            r.Id == id;

        var rentals = await _rentalRepository.GetAllAsync(filter,"EventActivity,ActivityType");
        var rental = _mapper.Map<OrderResponse>(rentals.FirstOrDefault());
        return rental;
    }

    public async Task<List<OrderResponse>?> GetAllRentalsAsync()
    {
        var result = await _rentalRepository.GetDbContext().Rentals.ToListAsync();
        return result.Count == 0 ? null : _mapper.Map<List<OrderResponse>>(result);
    }

    public async Task<dynamic> DeleteRentalAsync(int id)
    {
        var rental = _rentalRepository.GetDbContext().Rentals.FirstOrDefault(r => r.Id == id);
        if (rental == null) return null;
        
        rental.IsDeleted = true;
        await _rentalRepository.UpdateAsync(rental);

        return _mapper.Map<OrderResponse>(rental);
    }

    public async Task<PageListResponse<OrderResponse>> GetRentalByCustomerIdAsync(int customerId, int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 5;

        search = search?.ToLower();

        Expression<Func<Rental, bool>> filter = r =>
            r.AccountId == customerId &&
            (string.IsNullOrWhiteSpace(search) || r.EventName.ToLower().Contains(search));

        var rentals = await _rentalRepository.GetAllAsync(filter,"EventActivity,ActivityType");
        var totalPages = (int)Math.Ceiling(rentals.Count() / (double)pageSize);

        var paged = rentals
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = paged.Select(r => _mapper.Map<OrderResponse>(r)).ToList();
        
        return new PageListResponse<OrderResponse>
        {
            Items = result,
            TotalCount = totalPages,
            Page = page,
            PageSize = pageSize,
            HasNextPage = (page * pageSize) < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<OrderResponse?> CustomerSendRentalAsync(int rentalId)
    {
        Expression<Func<Rental, bool>> filter = r => r.Id == rentalId;
        var result = await _rentalRepository.GetAsync(filter);

        if (result == null) return null;

        result.Status = "Pending";
        
        await _rentalRepository.UpdateAsync(result);
        
        return _mapper.Map<OrderResponse>(result);
    }

    public async Task<List<OrderResponse>?> GetRentalsByCustomerAsync(int accountId)
    {
        var rentals = await _rentalRepository.GetDbContext().Rentals
            .Where(r => r.AccountId == accountId && r.IsDeleted == false)
            .ToListAsync();
        return rentals.Count == 0 ? null : _mapper.Map<List<OrderResponse>>(rentals);
    }

    public async Task<List<OrderResponse>> GetAllPendingRentalsAsync()
    {
        Expression<Func<Rental, bool>> filter = r => r.Status == "Pending";
        var rentals = await _rentalRepository.GetAllAsync(filter, "EventActivity,ActivityType");
        
        return rentals.ToList().Select(r => _mapper.Map<OrderResponse>(r)).ToList();
    }

    public async Task<OrderResponse?> ReceiveRequestAsync(int rentalId, int staffId)
    {
        Expression<Func<Rental, bool>> filter = r => r.Id == rentalId;

        var rental = await _rentalRepository.GetAsync(filter);
        
        if (rental == null) return null;
        
        rental.Status = "Received";
        rental.StaffId = staffId;
        
        await _rentalRepository.UpdateAsync(rental);
        
        return _mapper.Map<OrderResponse>(rental);
    }

    public async Task<List<OrderResponse>> GetAllReceivedRentalsByStaffId(int staffId)
    {
        Expression<Func<Rental, bool>> filter = r => r.Status == "Received" && r.StaffId == staffId;
        
        var rentals = await _rentalRepository.GetAllAsync(filter, "EventActivity,ActivityType");
        
        return rentals.ToList().Select(r => _mapper.Map<OrderResponse>(r)).ToList();
    }
}