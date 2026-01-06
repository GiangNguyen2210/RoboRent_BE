using AutoMapper;
using RoboRent_BE.Model.DTOs.ChecklistDelivery;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ChecklistDeliveryService :  IChecklistDeliveryService
{
    private readonly IChecklistDeliveryRepository _repository;
    private readonly IActualDeliveryRepository _actualDeliveryRepository;
    private readonly IChecklistDeliveryItemRepository _checklistDeliveryItemRepository;
    private readonly IMapper _mapper;
    
    public ChecklistDeliveryService(IChecklistDeliveryRepository repository,
        IActualDeliveryRepository actualDeliveryRepository,
        IMapper mapper,
        IChecklistDeliveryItemRepository checklistDeliveryItemRepository)
    {
        _repository = repository;
        _actualDeliveryRepository = actualDeliveryRepository;
        _mapper = mapper;
        _checklistDeliveryItemRepository = checklistDeliveryItemRepository;
    }
    
    public async Task<ChecklistDeliveryResponse?> CreateChecklistDeliveryAsync(ChecklistDeliveryRequest request)
    {
        var actualDelivery = await _actualDeliveryRepository.GetAsync(ad => ad.Id == request.ActualDeliveryId);

        if (actualDelivery == null) return null;

        ChecklistDelivery cd = _mapper.Map<ChecklistDelivery>(request);

        await _repository.AddAsync(cd);
        
        var res =await _repository.GetAsync(cd => cd.ActualDeliveryId == request.ActualDeliveryId);
        
        return _mapper.Map<ChecklistDeliveryResponse>(res); 
    }
    
    public async Task<dynamic> UpdateChecklistDeliveryAsync(ChecklistUpdateDeliveryRequest request)
{
    var checklistDeliveryId = request.checklistDeliveryId;

    if (!checklistDeliveryId.HasValue || checklistDeliveryId <= 0)
        throw new ArgumentException("checklistDeliveryId is required.");

    // 1) Load existing checklist (header)
    var existing = await _repository.GetAsync(cd => cd.Id == checklistDeliveryId.Value);
    if (existing == null) return null;

    // 2) Load existing items via item repository (because _repository.GetAsync may not Include)
    var existingItems = await _checklistDeliveryItemRepository
        .GetAllAsync(i => i.ChecklistDeliveryId == existing.Id);

    existing.Items = existingItems?.ToList() ?? new List<ChecklistDeliveryItem>();

    // 3) Map header fields onto existing entity
    _mapper.Map(request, existing);

    // Always set UpdatedAt on server (override DTO)
    existing.UpdatedAt = DateTime.UtcNow;

    // 4) Update items
    if (request.ChecklistDeliveryItemUpdateRequests != null &&
        request.ChecklistDeliveryItemUpdateRequests.Count > 0)
    {
        var itemMap = existing.Items.ToDictionary(i => i.Id);

        foreach (var dto in request.ChecklistDeliveryItemUpdateRequests)
        {
            if (!itemMap.TryGetValue(dto.Id, out var entityItem))
                throw new KeyNotFoundException($"ChecklistDeliveryItem not found: Id={dto.Id}");

            _mapper.Map(dto, entityItem);
            entityItem.UpdatedAt = DateTime.UtcNow;

            await _checklistDeliveryItemRepository.UpdateAsync(entityItem);
        }
    }

    // 5) Recalculate counts from items
    existing.TotalItems = existing.Items.Count;
    existing.PassItems  = existing.Items.Count(i => i.Result == ChecklistItemResult.Pass);
    existing.FailItems  = existing.Items.Count(i => i.Result == ChecklistItemResult.Fail);

    // 6) Save checklist header
    await _repository.UpdateAsync(existing);

    // 7) Return response (reload header + attach items if response needs them)
    var updated = await _repository.GetAsync(cd => cd.Id == existing.Id);
    if (updated == null) return null;

    updated.Items = existing.Items;

    return _mapper.Map<ChecklistDeliveryResponse>(updated);
}

}