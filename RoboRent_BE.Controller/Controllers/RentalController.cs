using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.RentalOrder;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Controller]
[Route("api/[controller]")]
public class RentalController : ControllerBase
{
    private readonly IRentalService  _rentalService;

    public RentalController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddRental([FromBody] CreateOrderRequest createOrderRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }
        
        var result = await _rentalService.CreateRentalAsync(createOrderRequest);

        if (result == null)
        {
            return BadRequest("Could not create new rental");
        }
        
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateRental([FromBody] UpdateOrderRequest updateOrderRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }
        
        var result = await _rentalService.UpdateRentalAsync(updateOrderRequest);

        if (result == null)
        {
            return BadRequest("Could not find valid data");
        }
        
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRentalsById(int id)
    {
        var result = await _rentalService.GetRentalAsync(id);

        if (result == null) return BadRequest("Could not find rental");
        
        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllRentalsAsync()
    {
        var result = await _rentalService.GetAllRentalsAsync();
        
        if (result == null) return BadRequest("There are no rentals exist");
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRentalById(int id)
    {
        var result = await _rentalService.DeleteRentalAsync(id);
        
        if (result == null) return BadRequest("Could not find rental");
        
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my-rentals/{accountId}")]
    public async Task<IActionResult> GetRentalsByCustomer(int accountId)
    {
        var result = await _rentalService.GetRentalsByCustomerAsync(accountId);
        return Ok(result ?? new List<OrderResponse>());
    }
}