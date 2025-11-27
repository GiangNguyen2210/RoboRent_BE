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
        try{
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
        catch (ArgumentException ex)   // ⬅⬅⬅ CATCH VALIDATION ERRORS PROPERLY
        {
            return BadRequest(new
            {
                success = false,
                errors = new List<string> { ex.Message }
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateRental([FromBody] UpdateOrderRequest updateOrderRequest)
    {
        try
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
        catch (ArgumentException ex)   // ⬅⬅⬅ CATCH VALIDATION ERRORS PROPERLY
        {
            return BadRequest(new
            {
                success = false,
                errors = new List<string> { ex.Message }
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
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

    // [Authorize]
    [HttpGet("my-rentals/{accountId}")]
    public async Task<IActionResult> GetRentalsByCustomer(int accountId)
    {
        var result = await _rentalService.GetRentalsByCustomerAsync(accountId);
        return Ok(result ?? new List<OrderResponse>());
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

    [HttpGet("staff/get/pending/rentals")]
    public async Task<IActionResult> GetAllPendingRentals()
    {
        try
        {
            var res = await _rentalService.GetAllPendingRentalsAsync();
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("staff/receive/{rentalId}/{staffId}")]
    public async Task<IActionResult> ReceiveRequest(int rentalId, int staffId)
    {
        try
        {
            var res = await _rentalService.ReceiveRequestAsync(rentalId, staffId);
            
            if (res == null) return BadRequest("Could not find rental");

            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpGet("staff/get/received/rentals/{staffId}")]
    public async Task<IActionResult> GetReceivedRequest(int staffId)
    {
        try
        {
            var res = await _rentalService.GetAllReceivedRentalsByStaffId(staffId);
            
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("staff/update/rental/{rentalId}")]
    public async Task<IActionResult> StaffUpdateRentalInfo(int rentalId, [FromBody] StaffUpdateRequest request)
    {
        try
        {
            var res = await _rentalService.StaffUpdateRentalInfoAsync(rentalId, request);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "Could not find rental"
            });
            
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("customer/cancel/rental/{rentalId}")]
    public async Task<IActionResult> CustomerCancelRental(int rentalId)
    {
        try
        {
            var res = await _rentalService.CustomerCancelRentalAsync(rentalId);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "Could not find rental"
            });
            
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }
    
    [HttpPut("customer/delete/rental/{rentalId}")]
    public async Task<IActionResult> CustomerDeleteRental(int rentalId)
    {
        try
        {
            var res = await _rentalService.CustomerDeleteRentalAsync(rentalId);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "Could not find rental"
            });
            
            return Ok(new
            {
                success = true,
                data = res
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }
    
    /// <summary>
    /// [STAFF/MANAGER] Mark rental as completed after event finished
    /// This will trigger Full payment creation
    /// </summary>
    [HttpPut("{rentalId}/complete")]
    public async Task<IActionResult> CompleteRental(int rentalId)
    {
        try
        {
            var result = await _rentalService.CompleteRentalAsync(rentalId);
        
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental not found"
                });
            }
        
            return Ok(new
            {
                success = true,
                message = "Rental completed successfully",
                data = result
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                success = false,
                message = e.Message
            });
        }
    }
}