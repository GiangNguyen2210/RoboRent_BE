using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.ContractTemplates;
using RoboRent_BE.Service.Interfaces;
using System.Security.Claims;

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

    [HttpPost("create-with-body")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> CreateWithBody([FromBody] CreateTemplateFromDefaultRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request", errors = ModelState });
            }

            var accountIdClaim = User.FindFirst("accountId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized(new { success = false, message = "Missing or invalid accountId in token" });
            }

            var createdByName = User.FindFirst(ClaimTypes.Name)?.Value;

            // Map to the service request, BodyJson intentionally omitted to force default loading
            var mapped = new CreateTemplateWithParsedClausesRequest
            {
                TemplateCode = request.TemplateCode,
                Title = request.Title,
                Description = request.Description,
                BodyJson = null,
                Version = request.Version
            };

            var result = await _contractTemplatesService.CreateFromBodyAndGenerateClausesAsync(mapped, accountId, createdByName);
            return Ok(new { success = true, data = result });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { success = false, message = e.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager, Staff, Admin")]
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
    [Authorize(Roles = "Manager, Staff, Admin")]
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
    [Authorize(Roles = "Manager, Staff, Admin")]
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
    [Authorize(Roles = "Manager, Staff, Admin")]
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
    [Authorize(Roles = "Manager, Admin")]
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
    [Authorize(Roles = "Manager, Admin")]
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
    [Authorize(Roles = "Manager, Admin")]
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
                message = "Contract template disabled successfully"
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

    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> ActivateContractTemplate(int id)
    {
        try
        {
            var result = await _contractTemplatesService.ActivateContractTemplateAsync(id);
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
                message = "Contract template activated successfully",
                data = result
            });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
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

