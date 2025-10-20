using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    public async Task<OrderResonse?> CreateRentalAsync(CreateOrderRequest createOrderRequest)
    {
        // Validate foreign keys first
        var accountExists = await _rentalRepository.GetDbContext().Accounts.AnyAsync(a => a.Id == createOrderRequest.AccountId);
        var eventExists = await _rentalRepository.GetDbContext().Events.AnyAsync(e => e.Id == createOrderRequest.EventId);
        var packageExists = await _rentalRepository.GetDbContext().RentalPackages.AnyAsync(p => p.Id == createOrderRequest.RentalPackageId);

        if (!accountExists || !eventExists || !packageExists)
            throw new ArgumentException("Invalid foreign key reference. Please check Account, Event, or RentalPackage ID.");

        Rental rental = _mapper.Map<Rental>(createOrderRequest);

        await _rentalRepository.AddAsync(rental);
        
        return rental == null ? null : _mapper.Map<OrderResonse>(rental);
    }

    public async Task<OrderResonse?> UpdateRentalAsync(UpdateOrderRequest updateOrderRequest)
    {
        // Validate foreign keys first
        var eventExists = await _rentalRepository.GetDbContext().Events.AnyAsync(e => e.Id == updateOrderRequest.EventId);
        var packageExists = await _rentalRepository.GetDbContext().RentalPackages.AnyAsync(p => p.Id == updateOrderRequest.RentalPackageId);

        if (!eventExists || !packageExists)
            return null;

        Rental? rental = await _rentalRepository.GetDbContext().Rentals.FirstOrDefaultAsync(r => r.Id == updateOrderRequest.Id);

        if (rental == null) return null;

        _mapper.Map(updateOrderRequest, rental);
        
        await _rentalRepository.UpdateAsync(rental);
        
        return _mapper.Map<OrderResonse>(rental);
    }

    public async Task<OrderResonse?> GetRentalAsync(int id)
    {
        var result = await _rentalRepository.GetDbContext().Rentals.FirstOrDefaultAsync(r => r.Id == id);
        return result == null ? null : _mapper.Map<OrderResonse>(result);
    }

    public async Task<List<OrderResonse>?> GetAllRentalsAsync()
    {
        var result = await _rentalRepository.GetDbContext().Rentals.ToListAsync();
        return result.Count == 0 ? null : _mapper.Map<List<OrderResonse>>(result);
    }

    public async Task<dynamic> DeleteRentalAsync(int id)
    {
        var rental = _rentalRepository.GetDbContext().Rentals.FirstOrDefault(r => r.Id == id);
        if (rental == null) return null;
        
        rental.IsDeleted = true;
        await _rentalRepository.UpdateAsync(rental);

        return _mapper.Map<OrderResonse>(rental);
    }
}