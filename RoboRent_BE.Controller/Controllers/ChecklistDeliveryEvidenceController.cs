using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.DTOs.ChecklistDeliveryEvidence;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Controller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChecklistDeliveryEvidenceController : ControllerBase
{
    private readonly IChecklistDeliveryEvidenceService  _checklistDeliveryEvidenceService;

    public ChecklistDeliveryEvidenceController(IChecklistDeliveryEvidenceService  checklistDeliveryEvidenceService)
    {
        _checklistDeliveryEvidenceService = checklistDeliveryEvidenceService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvidenceByCustomer(ChecklistDeliveryEvidenceCreateRequest request)
    {
        try
        {
            var res = await _checklistDeliveryEvidenceService.CreateEvidenceByCustomerAsync(request);
            
            if (res == null) return NotFound(new
            {
                success = false,
                message = "Not found"
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
}