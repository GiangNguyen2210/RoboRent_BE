using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.RentalContract;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalContractController : ControllerBase
{
    private readonly IRentalContractService _rentalContractService;

    public RentalContractController(IRentalContractService rentalContractService)
    {
        _rentalContractService = rentalContractService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRentalContracts()
    {
        try
        {
            var result = await _rentalContractService.GetAllRentalContractsAsync();
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
    public async Task<IActionResult> GetRentalContractById(int id)
    {
        try
        {
            var result = await _rentalContractService.GetRentalContractByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental contract not found"
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
    public async Task<IActionResult> GetRentalContractsByRentalId(int rentalId)
    {
        try
        {
            var result = await _rentalContractService.GetRentalContractsByRentalIdAsync(rentalId);
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
    public async Task<IActionResult> CreateRentalContract([FromBody] CreateRentalContractRequest request)
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
            var result = await _rentalContractService.CreateRentalContractAsync(request);
            return CreatedAtAction(nameof(GetRentalContractById), new { id = result.Id }, new
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
    public async Task<IActionResult> UpdateRentalContract(int id, [FromBody] UpdateRentalContractRequest request)
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
            var result = await _rentalContractService.UpdateRentalContractAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental contract not found"
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
    public async Task<IActionResult> DeleteRentalContract(int id)
    {
        try
        {
            var result = await _rentalContractService.DeleteRentalContractAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Rental contract not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Rental contract deleted successfully"
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
