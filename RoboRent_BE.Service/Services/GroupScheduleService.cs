using AutoMapper;
using RoboRent_BE.Model.DTOs.GroupSchedule;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class GroupScheduleService : IGroupScheduleService
{
    private readonly IGroupScheduleRepository _groupScheduleRepository;
    private readonly IActivityTypeGroupRepository _activityTypeGroupRepository;
    private readonly IMapper _mapper;
    private readonly IRentalRepository _rentalRepository;
    
    public GroupScheduleService(IRentalRepository rentalRepository,IGroupScheduleRepository groupScheduleRepository, IActivityTypeGroupRepository activityTypeGroupRepository,  IMapper mapper)
    {
        _groupScheduleRepository = groupScheduleRepository;
        _activityTypeGroupRepository = activityTypeGroupRepository;
        _mapper = mapper;
        _rentalRepository = rentalRepository;
    }
    
    public async Task<List<GroupScheduleGroupByDateResponse>> GetGroupScheduleByGroupId(int groupId)
    {
        // Lấy toàn bộ schedule trong group
        var schedules = await _groupScheduleRepository
            .GetAllAsync(gs => gs.ActivityTypeGroupId == groupId, "Rental,Rental.Staff,Rental.Account");

        // Group by EventDate
        var grouped = schedules
            .GroupBy(gs => gs.EventDate?.Date)
            .Select(g => new GroupScheduleGroupByDateResponse
            {
                EventDate = g.Key,
                Items = g.Select(x => _mapper.Map<GroupScheduleResponse>(x)).ToList()
            })
            .OrderBy(g => g.EventDate)
            .ToList();

        return grouped;
    }
    
    private static bool IsOverlapping(TimeOnly newDelivery, TimeOnly newFinish,
        TimeOnly existDelivery, TimeOnly existFinish)
    {
        return newDelivery < existFinish &&
               newFinish   > existDelivery;
    }

public async Task<GroupScheduleResponse?> CreateGroupSchedule(GroupScheduleCreateRequest request, int staffId)
{
    var group = await _activityTypeGroupRepository.GetAsync(g => g.Id == request.ActivityTypeGroupId);
    var rental = await _rentalRepository.GetAsync(r => r.Id == request.RentalId);
    var checkExist = await _groupScheduleRepository.GetAsync(gs => gs.RentalId == rental.Id);

    if (rental.StaffId != staffId)
        throw new Exception("The staff does not assign for this rental request.");
        
    if (checkExist != null)
        throw new Exception("Group Schedule already exist for this rental.");
        
    if (group == null || rental == null)
        return null;

    var newSchedule = _mapper.Map<GroupSchedule>(request);

    // Assign from rental
    newSchedule.EventCity = rental.City;
    newSchedule.EventDate = rental.EventDate;
    newSchedule.EventLocation = rental.Address;
    newSchedule.IsDeleted = false;
    newSchedule.Status = "planned";
    newSchedule.RentalId = rental.Id;

    // ================================
    // ✅ Validate timeline order
    // ================================
    if (!(newSchedule.DeliveryTime < newSchedule.StartTime
          && newSchedule.StartTime < newSchedule.EndTime
          && newSchedule.EndTime < newSchedule.FinishTime))
    {
        throw new Exception("Ensure DeliveryTime < StartTime < EndTime < FinishTime");
    }

    var newDelivery = newSchedule.DeliveryTime!.Value;
    var newFinish   = newSchedule.FinishTime!.Value;

    // Get schedules same group + same day
    var existingSchedules = await _groupScheduleRepository.GetAllAsync(
        gs => gs.ActivityTypeGroupId == request.ActivityTypeGroupId
              && gs.EventDate == newSchedule.EventDate
              && !gs.IsDeleted
    );

    // Overlap check
    foreach (var item in existingSchedules)
    {
        if (IsOverlapping(newDelivery, newFinish,
                item.DeliveryTime!.Value, item.FinishTime!.Value))
        {
            throw new Exception(
                $"Lịch bị trùng với lịch ID {item.Id} " +
                $"({item.DeliveryTime} - {item.FinishTime})"
            );
        }
    }

    await _groupScheduleRepository.AddAsync(newSchedule);

    return _mapper.Map<GroupScheduleResponse>(newSchedule);
}


    public async Task<GroupScheduleResponse?> UpdateGroupSchedule(int scheduleId, GroupScheduleUpdateRequest request)
    {
        var schedule = await _groupScheduleRepository.GetAsync(gs => gs.Id == scheduleId);
        var rental = await _rentalRepository.GetAsync(r => r.Id == request.RentalId);
        var group = await _activityTypeGroupRepository.GetAsync(g => g.Id == request.ActivityTypeGroupId);

        if (schedule == null || rental == null || group == null)
            return null;

        // Map dữ liệu mới
        _mapper.Map(request, schedule);

        schedule.EventCity = rental.City;
        schedule.EventDate = rental.EventDate;
        schedule.EventLocation = rental.Address;
        schedule.ActivityTypeGroupId = request.ActivityTypeGroupId;

        var newDelivery = schedule.DeliveryTime!.Value;
        var newFinish   = schedule.FinishTime!.Value;

        // Lấy tất cả lịch cùng ngày + groupId, nhưng bỏ chính nó
        var existingSchedules = await _groupScheduleRepository.GetAllAsync(
            gs => gs.ActivityTypeGroupId == request.ActivityTypeGroupId
                  && gs.EventDate == schedule.EventDate
                  && gs.Id != scheduleId        // ❗ TRÁNH TỰ TRÙNG CHÍNH NÓ
                  && !gs.IsDeleted
        );

        // Check trùng
        foreach (var item in existingSchedules)
        {
            if (IsOverlapping(newDelivery, newFinish,
                    item.DeliveryTime!.Value, item.FinishTime!.Value))
            {
                throw new Exception(
                    $"Không thể cập nhật vì trùng với lịch ID {item.Id} " +
                    $"({item.DeliveryTime} - {item.FinishTime})"
                );
            }
        }

        await _groupScheduleRepository.UpdateAsync(schedule);

        return _mapper.Map<GroupScheduleResponse>(schedule);
    }

    public async Task<GroupScheduleResponse?> CancelGroupScheduleById(int scheduleId)
    {
        var gs = await _groupScheduleRepository.GetAsync(gs => gs.Id == scheduleId);
        
        return gs == null ? null : _mapper.Map<GroupScheduleResponse>(gs);
    }
}