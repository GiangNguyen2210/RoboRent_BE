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
    private readonly IAccountRepository _accountRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IGroupScheduleRepository _groupScheduleRepository;
    
    public ChecklistDeliveryService(IChecklistDeliveryRepository repository,
        IActualDeliveryRepository actualDeliveryRepository,
        IMapper mapper,
        IChecklistDeliveryItemRepository checklistDeliveryItemRepository,
        IAccountRepository accountRepository,
        IRentalRepository rentalRepository,
        IGroupScheduleRepository groupScheduleRepository)
    {
        _repository = repository;
        _actualDeliveryRepository = actualDeliveryRepository;
        _mapper = mapper;
        _checklistDeliveryItemRepository = checklistDeliveryItemRepository;
        _accountRepository = accountRepository;
        _rentalRepository = rentalRepository;
        _groupScheduleRepository = groupScheduleRepository;
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

    if (request.OverallResult == ChecklistItemResult.Pass)
    {
        existing.Status = ChecklistDeliveryStatus.Approved;
    }
    else
    {
        existing.Status = ChecklistDeliveryStatus.Draft ;
    }

    // 6) Save checklist header
    await _repository.UpdateAsync(existing);

    // 7) Return response (reload header + attach items if response needs them)
    var updated = await _repository.GetAsync(cd => cd.Id == existing.Id);
    if (updated == null) return null;

    updated.Items = existing.Items;

    return _mapper.Map<ChecklistDeliveryResponse>(updated);
}

    public async Task<ChecklistDeliveryResponse?> GetChecklistDeliveryByActAsync(int actId)
    {
        var actualDelivery = await _actualDeliveryRepository.GetAsync(ad => ad.Id == actId);
        
        if (actualDelivery == null) return null;
        
        var res = await _repository.GetAsync(r => r.ActualDeliveryId == actualDelivery.Id);
        
        return _mapper.Map<ChecklistDeliveryResponse>(res);
    }

    public async Task<ChecklistDeliveryResponse?> CustomerConfirmChecklistDelivery(int checklistDeliveryId, CustomerConfirmRequest customerConfirmRequest)
    {
        var checklistDelivery = await _repository.GetAsync(ad => ad.Id == checklistDeliveryId);

        var customer = await _accountRepository.GetAsync(a => a.Id == customerConfirmRequest.CustomerAcceptedById);

        if (checklistDelivery == null || customer == null) return null;
        
        _mapper.Map(customerConfirmRequest, checklistDelivery);
        
        await _repository.UpdateAsync(checklistDelivery);
        
        return _mapper.Map<ChecklistDeliveryResponse>(checklistDelivery);
    }

    public async Task<int?> CustomerGetChecklistDeliveryByRentalIdAsync(int rentalId)
    {
        var rental = await _rentalRepository.GetAsync(r => r.Id == rentalId);
        
        if (rental == null) return null;

        var groupSchdule = await _groupScheduleRepository.GetAsync(gs => gs.RentalId == rental.Id);
        
        if (groupSchdule == null) return null;

        var actualDelivery = await _actualDeliveryRepository.GetAsync(ad => ad.GroupScheduleId == groupSchdule.Id);
        
        if (actualDelivery == null) return null;
        
        return actualDelivery.Id;
    }

    public async Task<dynamic> StaffConfirmPickupRobotAsync(int checklistDeliveryId)
    {
        var checklistDelivery = await _repository.GetAsync(cd => cd.Id == checklistDeliveryId);
        
        if (checklistDelivery == null) return null;

        var ac = await _actualDeliveryRepository.GetAsync(a => a.Id == checklistDelivery.ActualDeliveryId);
        
        if (ac == null) return null;
        
        ac.ActualPickupTime = DateTime.UtcNow;

        await _actualDeliveryRepository.UpdateAsync(ac);
        
        checklistDelivery.Status = ChecklistDeliveryStatus.Completed;
        
        await _repository.UpdateAsync(checklistDelivery);
        
        return _mapper.Map<ChecklistDeliveryResponse>(checklistDelivery);
    }
}