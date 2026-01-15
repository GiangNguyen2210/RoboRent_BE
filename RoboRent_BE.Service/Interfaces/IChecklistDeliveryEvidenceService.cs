using RoboRent_BE.Model.DTOs.ChecklistDeliveryEvidence;

namespace RoboRent_BE.Service.Interfaces;

public interface IChecklistDeliveryEvidenceService
{
    public Task<ChecklistDeliveryEvidenceResponse?> CreateEvidenceByCustomerAsync(ChecklistDeliveryEvidenceCreateRequest request);
}