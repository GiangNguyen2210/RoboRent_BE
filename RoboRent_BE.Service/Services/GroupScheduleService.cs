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
            .GetAllAsync(gs => gs.ActivityTypeGroupId == groupId && gs.IsDeleted == false, "Rental,Rental.Staff,Rental.Account");

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
        var existing = await _groupScheduleRepository.GetByRentalAsync(request.RentalId.Value);

        if (group == null || rental == null)
            throw new Exception("Invalid group or rental.");

        if (rental.StaffId != staffId)
            throw new Exception("The staff is not assigned to this rental.");

        GroupSchedule schedule;

        // A — Active schedule exists → BLOCK
        if (existing != null && existing.IsDeleted == false)
            throw new Exception("Group Schedule already exist for this rental.");

        if (rental.ActivityTypeId != group.ActivityTypeId)
        {
            throw new Exception("The group is not assigned to this rental's activity type.");
        }
        
        // B — Soft-deleted schedule exists → RESTORE
        if (existing != null && existing.IsDeleted == true)
        {
            schedule = existing;

            _mapper.Map(request, schedule);            // update fields
            schedule.IsDeleted = false;
            schedule.Status = "planned";

            // attach existing for EF
            _groupScheduleRepository.Attach(schedule);
            await _groupScheduleRepository.UpdateNotGenericAsync(schedule);
        }
        else
        {
            // C — Create new schedule
            schedule = _mapper.Map<GroupSchedule>(request);
            schedule.IsDeleted = false;
            schedule.Status = "planned";

            await _groupScheduleRepository.AddAsync(schedule);
        }

        // Auto-fill from rental
        schedule.EventCity = rental.City;
        schedule.EventDate = rental.EventDate;
        schedule.EventLocation = rental.Address;
        schedule.RentalId = rental.Id;

        // Timeline validation
        if (!(schedule.DeliveryTime < schedule.StartTime &&
              schedule.StartTime < schedule.EndTime &&
              schedule.EndTime < schedule.FinishTime))
        {
            throw new Exception("Ensure DeliveryTime < StartTime < EndTime < FinishTime");
        }

        // Overlap check ONLY on new or restored
        if (existing == null || existing.IsDeleted == true)
        {
            var sameDay = await _groupScheduleRepository.GetSameDaySchedulesAsync(
                request.ActivityTypeGroupId.Value,
                schedule.EventDate.Value
            );

            foreach (var item in sameDay)
            {
                if (IsOverlapping(schedule.DeliveryTime.Value, schedule.FinishTime.Value,
                                  item.DeliveryTime.Value, item.FinishTime.Value))
                {
                    throw new Exception($"Overlap with schedule of event {item.Rental.EventName} " + $"({item.DeliveryTime} - {item.FinishTime})");
                }
            }
        }

        rental.Status = "Scheduled";

        await _rentalRepository.UpdateAsync(rental);
        
        await _groupScheduleRepository.SaveChangesAsync();

        return _mapper.Map<GroupScheduleResponse>(schedule);
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
                    $"Overlap with schedule of event {item.Rental.EventName} " +
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

    public async Task<GroupScheduleResponse?> CustomerGetGroupScheduleByRentalId(int rentalId)
    {
        var res = await _groupScheduleRepository.GetAsync(gs => gs.RentalId == rentalId && gs.IsDeleted == false, "Rental,Rental.Staff,Rental.Account");
        
        if (res == null) return null;
        
        return _mapper.Map<GroupScheduleResponse>(res);
    }
}