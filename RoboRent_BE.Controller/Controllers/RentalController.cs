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

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetRentalByCustomerId(int customerId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _rentalService.GetRentalByCustomerIdAsync(customerId, page, pageSize, search);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("customer/send/{rentalId}")]
    public async Task<IActionResult> SendRequest(int rentalId)
    {
        try
        {
            var result = await _rentalService.CustomerSendRentalAsync(rentalId);
            
            if (result == null) return NotFound("Could not found rental");
            
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}