 using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalDetailController : ControllerBase
{
    private readonly IRentalDetailService _rentalDetailService;

    public RentalDetailController(IRentalDetailService rentalDetailService)
    {
        _rentalDetailService = rentalDetailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRentalDetails()
    {
        try
        {
            var result = await _rentalDetailService.GetAllRentalDetailsAsync();
            return Ok(new
            {
                success = true,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRentalDetailById(int id)
    {
        try
        {
            var result = await _rentalDetailService.GetRentalDetailByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental detail not found"
                });
            }

            return Ok(new
            {
                success = true,
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

    [HttpGet("rental/{rentalId}")]
    public async Task<IActionResult> GetRentalDetailsByRentalId(int rentalId)
    {
        try
        {
            var result = await _rentalDetailService.GetRentalDetailsByRentalIdAsync(rentalId);
            return Ok(new
            {
                success = true,
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

    [HttpGet("robotype/{roboTypeId}")]
    public async Task<IActionResult> GetRentalDetailsByRoboTypeId(int roboTypeId)
    {
        try
        {
            var result = await _rentalDetailService.GetRentalDetailsByRoboTypeIdAsync(roboTypeId);
            return Ok(new
            {
                success = true,
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

    [HttpPost]
    public async Task<IActionResult> CreateRentalDetail([FromBody] CreateRentalDetailRequest request)
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

        try
        {
            var result = await _rentalDetailService.CreateRentalDetailAsync(request);
            return CreatedAtAction(nameof(GetRentalDetailById), new { id = result.Id }, new
            {
                success = true,
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRentalDetail(int id, [FromBody] UpdateRentalDetailRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new
            {
                success = false,
                message = "ID mismatch"
            });
        }

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

        try
        {
            var result = await _rentalDetailService.UpdateRentalDetailAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental detail not found"
                });
            }

            return Ok(new
            {
                success = true,
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRentalDetail(int id)
    {
        try
        {
            var result = await _rentalDetailService.DeleteRentalDetailAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental detail not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Rental detail deleted successfully"
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


