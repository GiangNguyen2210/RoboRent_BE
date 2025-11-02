using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOS.DraftClauses;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DraftClausesController : ControllerBase
{
    private readonly IDraftClausesService _draftClausesService;

    public DraftClausesController(IDraftClausesService draftClausesService)
    {
        _draftClausesService = draftClausesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDraftClauses()
    {
        try
        {
            var result = await _draftClausesService.GetAllDraftClausesAsync();
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
    public async Task<IActionResult> GetDraftClausesById(int id)
    {
        try
        {
            var result = await _draftClausesService.GetDraftClausesByIdAsync(id);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Draft clause not found"
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

    [HttpGet("contract-draft/{contractDraftId}")]
    public async Task<IActionResult> GetDraftClausesByContractDraftId(int contractDraftId)
    {
        try
        {
            var result = await _draftClausesService.GetDraftClausesByContractDraftIdAsync(contractDraftId);
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

    [HttpGet("template-clause/{templateClauseId}")]
    public async Task<IActionResult> GetDraftClausesByTemplateClauseId(int templateClauseId)
    {
        try
        {
            var result = await _draftClausesService.GetDraftClausesByTemplateClauseIdAsync(templateClauseId);
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

    [HttpGet("contract-draft/{contractDraftId}/modified/{isModified}")]
    public async Task<IActionResult> GetDraftClausesByIsModified(int contractDraftId, bool isModified)
    {
        try
        {
            var result = await _draftClausesService.GetDraftClausesByIsModifiedAsync(contractDraftId, isModified);
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
    public async Task<IActionResult> CreateDraftClauses([FromBody] CreateDraftClausesRequest request)
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
            var result = await _draftClausesService.CreateDraftClausesAsync(request);
            return CreatedAtAction(nameof(GetDraftClausesById), new { id = result.Id }, new
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

    [HttpPost("custom")]
    public async Task<IActionResult> CreateCustomDraftClause([FromBody] CreateCustomDraftClauseRequest request)
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
            var result = await _draftClausesService.CreateCustomDraftClauseAsync(request);
            return CreatedAtAction(nameof(GetDraftClausesById), new { id = result.DraftClause.Id }, new
            {
                success = true,
                data = result,
                message = result.Message
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
    public async Task<IActionResult> UpdateDraftClauses(int id, [FromBody] UpdateDraftClausesRequest request)
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
            var result = await _draftClausesService.UpdateDraftClausesAsync(request);
            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Draft clause not found"
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
    public async Task<IActionResult> DeleteDraftClauses(int id)
    {
        try
        {
            var result = await _draftClausesService.DeleteDraftClausesAsync(id);
            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Draft clause not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Draft clause deleted successfully"
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

