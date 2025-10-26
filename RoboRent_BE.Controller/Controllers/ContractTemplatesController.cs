using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.ContractTemplates;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContractTemplatesController : ControllerBase
{
    private readonly IContractTemplatesService _contractTemplatesService;

    public ContractTemplatesController(IContractTemplatesService contractTemplatesService)
    {
        _contractTemplatesService = contractTemplatesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContractTemplates()
    {
        try
        {
            var result = await _contractTemplatesService.GetAllContractTemplatesAsync();
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
    public async Task<IActionResult> GetContractTemplatesById(int id)
    {
        try
        {
            var result = await _contractTemplatesService.GetContractTemplatesByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract template not found"
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

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetContractTemplatesByStatus(string status)
    {
        try
        {
            var result = await _contractTemplatesService.GetContractTemplatesByStatusAsync(status);
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

    [HttpGet("created-by/{createdBy}")]
    public async Task<IActionResult> GetContractTemplatesByCreatedBy(int createdBy)
    {
        try
        {
            var result = await _contractTemplatesService.GetContractTemplatesByCreatedByAsync(createdBy);
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

    [HttpGet("version/{version}")]
    public async Task<IActionResult> GetContractTemplatesByVersion(string version)
    {
        try
        {
            var result = await _contractTemplatesService.GetContractTemplatesByVersionAsync(version);
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
    public async Task<IActionResult> CreateContractTemplates([FromBody] CreateContractTemplatesRequest request)
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
            var result = await _contractTemplatesService.CreateContractTemplatesAsync(request);
            return CreatedAtAction(nameof(GetContractTemplatesById), new { id = result.Id }, new
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
    public async Task<IActionResult> UpdateContractTemplates(int id, [FromBody] UpdateContractTemplatesRequest request)
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
            var result = await _contractTemplatesService.UpdateContractTemplatesAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract template not found"
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
    public async Task<IActionResult> DeleteContractTemplates(int id)
    {
        try
        {
            var result = await _contractTemplatesService.DeleteContractTemplatesAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Contract template not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Contract template deleted successfully"
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
