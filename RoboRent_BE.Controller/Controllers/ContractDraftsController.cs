using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContractDraftsController : ControllerBase
{
    private readonly IContractDraftsService _contractDraftsService;

    public ContractDraftsController(IContractDraftsService contractDraftsService)
    {
        _contractDraftsService = contractDraftsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContractDrafts()
    {
        try
        {
            var result = await _contractDraftsService.GetAllContractDraftsAsync();
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
    public async Task<IActionResult> GetContractDraftsById(int id)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
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
    public async Task<IActionResult> GetContractDraftsByRentalId(int rentalId)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByRentalIdAsync(rentalId);
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

    [HttpGet("staff/{staffId}")]
    public async Task<IActionResult> GetContractDraftsByStaffId(int staffId)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByStaffIdAsync(staffId);
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

    [HttpGet("manager/{managerId}")]
    public async Task<IActionResult> GetContractDraftsByManagerId(int managerId)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByManagerIdAsync(managerId);
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

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetContractDraftsByStatus(string status)
    {
        try
        {
            var result = await _contractDraftsService.GetContractDraftsByStatusAsync(status);
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
    public async Task<IActionResult> CreateContractDrafts([FromBody] CreateContractDraftsRequest request)
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
            var result = await _contractDraftsService.CreateContractDraftsAsync(request);
            return CreatedAtAction(nameof(GetContractDraftsById), new { id = result.Id }, new
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
    public async Task<IActionResult> UpdateContractDrafts(int id, [FromBody] UpdateContractDraftsRequest request)
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
            var result = await _contractDraftsService.UpdateContractDraftsAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
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
    public async Task<IActionResult> DeleteContractDrafts(int id)
    {
        try
        {
            var result = await _contractDraftsService.DeleteContractDraftsAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract draft not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Contract draft deleted successfully"
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
