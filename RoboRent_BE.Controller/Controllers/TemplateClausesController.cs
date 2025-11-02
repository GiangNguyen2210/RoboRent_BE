using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.TemplateClauses;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TemplateClausesController : ControllerBase
{
    private readonly ITemplateClausesService _templateClausesService;

    public TemplateClausesController(ITemplateClausesService templateClausesService)
    {
        _templateClausesService = templateClausesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTemplateClauses()
    {
        try
        {
            var result = await _templateClausesService.GetAllTemplateClausesAsync();
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
    public async Task<IActionResult> GetTemplateClausesById(int id)
    {
        try
        {
            var result = await _templateClausesService.GetTemplateClausesByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Template clause not found"
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

    [HttpGet("contract-template/{contractTemplateId}")]
    public async Task<IActionResult> GetTemplateClausesByContractTemplateId(int contractTemplateId)
    {
        try
        {
            var result = await _templateClausesService.GetTemplateClausesByContractTemplateIdAsync(contractTemplateId);
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

    [HttpGet("mandatory/{isMandatory}")]
    public async Task<IActionResult> GetTemplateClausesByIsMandatory(bool isMandatory)
    {
        try
        {
            var result = await _templateClausesService.GetTemplateClausesByIsMandatoryAsync(isMandatory);
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

    [HttpGet("editable/{isEditable}")]
    public async Task<IActionResult> GetTemplateClausesByIsEditable(bool isEditable)
    {
        try
        {
            var result = await _templateClausesService.GetTemplateClausesByIsEditableAsync(isEditable);
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

    [HttpGet("available/{contractTemplateId}/{contractDraftId}")]
    public async Task<IActionResult> GetAvailableTemplateClausesForDraft(int contractTemplateId, int contractDraftId)
    {
        try
        {
            var result = await _templateClausesService.GetAvailableTemplateClausesForDraftAsync(contractTemplateId, contractDraftId);
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
    public async Task<IActionResult> CreateTemplateClauses([FromBody] CreateTemplateClausesRequest request)
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
            var result = await _templateClausesService.CreateTemplateClausesAsync(request);
            return CreatedAtAction(nameof(GetTemplateClausesById), new { id = result.Id }, new
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
    public async Task<IActionResult> UpdateTemplateClauses(int id, [FromBody] UpdateTemplateClausesRequest request)
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
            var result = await _templateClausesService.UpdateTemplateClausesAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Template clause not found"
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
    public async Task<IActionResult> DeleteTemplateClauses(int id)
    {
        try
        {
            var result = await _templateClausesService.DeleteTemplateClausesAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Template clause not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Template clause deleted successfully"
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

