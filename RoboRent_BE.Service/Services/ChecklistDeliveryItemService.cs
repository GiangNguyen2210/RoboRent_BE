using AutoMapper;
using RoboRent_BE.Model.DTOs.ChecklistDeliveryItem;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ChecklistDeliveryItemService :  IChecklistDeliveryItemService
{
    private readonly IChecklistDeliveryItemRepository _repository;
    private readonly IChecklistDeliveryRepository _checklistDeliveryRepository;
    private readonly IChecklistDeliveryItemTemplateRepository _checklistDeliveryItemTemplateRepository;
    private readonly IMapper _mapper;

    public ChecklistDeliveryItemService(IChecklistDeliveryItemRepository repository,
        IChecklistDeliveryRepository checklistDeliveryRepository,
        IChecklistDeliveryItemTemplateRepository checklistDeliveryItemTemplateRepository,
        IMapper mapper)
    {
        _repository = repository;
        _checklistDeliveryRepository = checklistDeliveryRepository;
        _checklistDeliveryItemTemplateRepository = checklistDeliveryItemTemplateRepository;
        _mapper = mapper;
    }
    
    public async Task<List<ChecklistDeliveryItemResponse>?> CreateItemAsync(int checklistDeliveryId)
{
    var checklist = await _checklistDeliveryRepository.GetAsync(x => x.Id == checklistDeliveryId);
    if (checklist == null) return null;

    // 1) Nếu checklist đã có item rồi thì không tạo lại (tránh duplicate)
    // (Giả sử repository của item có hàm GetAllAsync theo predicate; nếu chưa có thì bạn bổ sung)
    var existedItems = (await _repository.GetAllAsync(i => i.ChecklistDeliveryId == checklistDeliveryId)).ToList();
    if (existedItems.Count > 0)
        return existedItems.Select(x => _mapper.Map<ChecklistDeliveryItemResponse>(x)).ToList();

    // 2) Xác định filter template theo RoboTypeId (và ActivityTypeId nếu bạn có)
    // Hiện tại ChecklistDelivery của bạn không show field RoboTypeId/ActivityTypeId ở đây,
    // nên mình lấy theo:
    // - template.RoboTypeId == null (common)
    // - hoặc template.RoboTypeId == checklist.RoboTypeId (nếu checklist có field đó)
    // Nếu ChecklistDelivery chưa có RoboTypeId, bạn có thể truyền roboTypeId vào CreateItemAsync
    int? roboTypeId = null;

    // ✅ Nếu ChecklistDelivery của bạn có RoboTypeId thì bật dòng này:
    // roboTypeId = checklist.RoboTypeId;

    int? activityTypeId = null;
    // ✅ Nếu ChecklistDelivery của bạn có ActivityTypeId thì bật dòng này:
    // activityTypeId = checklist.ActivityTypeId;

    var templates = (await _checklistDeliveryItemTemplateRepository.GetAllAsync(t =>
            t.IsActive &&
            (t.RoboTypeId == null || (roboTypeId != null && t.RoboTypeId == roboTypeId)) &&
            (t.ActivityTypeId == null || (activityTypeId != null && t.ActivityTypeId == activityTypeId))
        ))
        .OrderBy(t => t.Group)
        .ThenBy(t => t.SortOrder)
        .ToList();

    if (templates.Count == 0)
        return new List<ChecklistDeliveryItemResponse>(); // không có template thì trả list rỗng

    // 3) Clone template -> ChecklistDeliveryItem
    var now = DateTime.UtcNow;

    var newItems = templates.Select(t => new ChecklistDeliveryItem
    {
        ChecklistDeliveryId = checklistDeliveryId,

        // Identity
        Key = t.Code,
        Label = t.Title,
        Category = t.Group.ToString(),

        // Severity: bạn chưa show enum mapping, mình set mặc định
        Severity = t.IsCritical ? ChecklistSeverity.High : ChecklistSeverity.Medium,

        // Rules
        IsRequired = t.IsCritical, // hoặc true nếu bạn muốn mọi item đều required
        EvidenceRequiredOnFail = t.FailRequiresEvidence,
        MustPassToDispatch = t.IsCritical,

        SortOrder = t.SortOrder,

        // Expected/value type
        Expected = t.RequiresMeasuredValue
            ? (string.IsNullOrWhiteSpace(t.MeasuredValueLabel) ? "Provide measured value" : $"Provide: {t.MeasuredValueLabel}")
            : null,

        ValueType = t.RequiresMeasuredValue ? "text" : "select",

        // Values
        Result = ChecklistItemResult.Unknown,
        Note = null,

        CreatedAt = now,
        UpdatedAt = now
    }).ToList();

    // 4) Save
    // Nếu repository bạn có AddRangeAsync thì dùng AddRangeAsync.
    // Nếu không có, bạn có thể foreach AddAsync.
    if (_repository is IChecklistDeliveryItemRepository repoWithRange)
    {
        // nếu bạn có AddRangeAsync trong interface thì gọi thẳng
        await repoWithRange.AddRangeAsync(newItems);
    }
    else
    {
        foreach (var item in newItems)
            await _repository.AddAsync(item);
    }
    
    return newItems.Select(x => _mapper.Map<ChecklistDeliveryItemResponse>(x)).ToList();
}

    public async Task<List<ChecklistDeliveryItemResponse>?> GetAllChecklistDeliveryItemsAsync(int checklistDeliveryId)
    {
        var checklist = await _checklistDeliveryRepository.GetAsync(x => x.Id == checklistDeliveryId);
        if (checklist == null) return null;
        
        var res = await _repository.GetAllAsync(r => r.ChecklistDeliveryId == checklistDeliveryId);
        
        return res.Select(x => _mapper.Map<ChecklistDeliveryItemResponse>(x)).ToList();
    }

}