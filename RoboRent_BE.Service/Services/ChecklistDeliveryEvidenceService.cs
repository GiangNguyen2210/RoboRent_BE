using AutoMapper;
using RoboRent_BE.Model.DTOs.ChecklistDeliveryEvidence;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ChecklistDeliveryEvidenceService : IChecklistDeliveryEvidenceService
{
    private readonly IChecklistDeliveryEvidenceRepository _checklistDeliveryEvidenceRepository;
    private readonly IChecklistDeliveryRepository _checklistDeliveryRepository;
    private readonly IChecklistDeliveryItemRepository _checklistDeliveryItemRepository;
    private readonly IMapper _mapper;
    
    public ChecklistDeliveryEvidenceService(IChecklistDeliveryEvidenceRepository checklistDeliveryEvidenceRepository,
        IChecklistDeliveryRepository checklistDeliveryRepository,
        IChecklistDeliveryItemRepository checklistDeliveryItemRepository)
    {
        _checklistDeliveryEvidenceRepository = checklistDeliveryEvidenceRepository;
        _checklistDeliveryRepository = checklistDeliveryRepository;
        _checklistDeliveryItemRepository = checklistDeliveryItemRepository;
    }
    public async Task<ChecklistDeliveryEvidenceResponse?> CreateEvidenceByCustomerAsync(ChecklistDeliveryEvidenceCreateRequest request)
    {
        var checklistDelivery = await _checklistDeliveryRepository.GetAsync(a => a.Id == request.ChecklistDeliveryId);

        var checklistDeliveryItem = await _checklistDeliveryItemRepository.GetAsync(a => a.Id == request.ChecklistDeliveryItemId);
        
        if (checklistDelivery == null || checklistDeliveryItem == null) return null;

        var createdChecklistDeliveryEvidence = _mapper.Map<ChecklistDeliveryEvidence>(request);

        var res = await _checklistDeliveryEvidenceRepository.AddAsync(createdChecklistDeliveryEvidence);

        return _mapper.Map<ChecklistDeliveryEvidenceResponse>(res);

    }
}