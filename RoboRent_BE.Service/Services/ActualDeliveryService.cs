using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.DTOs.ActualDelivery;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Enums;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;
using RoboRent_BE.Service.Configuration;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ActualDeliveryService : IActualDeliveryService
{
    private readonly IActualDeliveryRepository _deliveryRepo;
    private readonly IGroupScheduleRepository _groupScheduleRepo;

    public ActualDeliveryService(
        IActualDeliveryRepository deliveryRepo,
        IGroupScheduleRepository groupScheduleRepo)
    {
        _deliveryRepo = deliveryRepo;
        _groupScheduleRepo = groupScheduleRepo;
    }

    public async Task<ActualDeliveryResponse> CreateActualDeliveryAsync(CreateActualDeliveryRequest request)
    {
        // Validate GroupSchedule exists
        var groupSchedule = await _groupScheduleRepo.GetAsync(
            gs => gs.Id == request.GroupScheduleId,
            includeProperties: "Rental,Rental.Account,Rental.ActivityType"
        );

        if (groupSchedule == null)
        {
            throw new Exception($"GroupSchedule {request.GroupScheduleId} not found");
        }

        if (groupSchedule.Status != "planned")
        {
            throw new Exception($"Cannot create delivery. GroupSchedule status must be 'planned'. Current: {groupSchedule.Status}");
        }

        // Check if ActualDelivery already exists for this GroupSchedule
        var existing = await _deliveryRepo.GetByGroupScheduleIdAsync(request.GroupScheduleId);
        if (existing != null)
        {
            throw new Exception($"ActualDelivery already exists for GroupSchedule {request.GroupScheduleId}");
        }

        // Change GroupSchedule status to "scheduled"
        groupSchedule.Status = "scheduled";
        await _groupScheduleRepo.UpdateAsync(groupSchedule);

        // Create ActualDelivery
        var delivery = new ActualDelivery
        {
            GroupScheduleId = request.GroupScheduleId,
            StaffId = null,
            Type = request.Type,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _deliveryRepo.AddAsync(delivery);

        return await MapToResponseAsync(delivery.Id);
    }

    public async Task<ActualDeliveryResponse> AssignStaffAsync(int deliveryId, AssignStaffRequest request)
    {
        var delivery = await _deliveryRepo.GetWithDetailsAsync(deliveryId);

        if (delivery == null)
        {
            throw new Exception($"ActualDelivery {deliveryId} not found");
        }

        if (delivery.Status != "Pending")
        {
            throw new Exception($"Cannot assign staff. Delivery status must be 'Pending'. Current: {delivery.Status}");
        }

        // Check staff conflict
        var conflictCheck = await CheckStaffConflictAsync(request.StaffId, delivery.GroupScheduleId);
        if (conflictCheck.HasConflict)
        {
            var conflictMessages = conflictCheck.Conflicts
                .Select(c => $"- {c.EventName} ({c.ScheduledStart:g} - {c.ScheduledEnd:g})")
                .ToList();

            throw new Exception(
                $"Staff {request.StaffId} has schedule conflict:\n" +
                string.Join("\n", conflictMessages)
            );
        }

        // 🆕 Auto-classify DeliveryType based on staff's deliveries on same day
        var eventDate = delivery.GroupSchedule.EventDate?.Date ?? DateTime.UtcNow.Date;
        var staffDeliveriesOnSameDay = await _deliveryRepo.GetByStaffAndDateRangeAsync(
            request.StaffId,
            eventDate,
            eventDate
        );

        // FirstOfDay if this is the first delivery, otherwise MidDay
        // LastOfDay will be handled separately (when manager marks it or auto-detect at end of day)
        if (staffDeliveriesOnSameDay.Count == 0)
        {
            delivery.Type = DeliveryType.FirstOfDay;
        }
        else
        {
            delivery.Type = DeliveryType.MidDay;
        }

        // Assign staff
        delivery.StaffId = request.StaffId;
        delivery.Status = "Assigned";
        delivery.Notes = request.Notes;
        delivery.UpdatedAt = DateTime.UtcNow;

        await _deliveryRepo.UpdateAsync(delivery);

        // 🆕 Re-classify all deliveries for this staff on this day
        await ReclassifyDeliveriesOfDayAsync(request.StaffId, eventDate);

        // Fetch updated delivery to return correct Type
        var updatedDelivery = await _deliveryRepo.GetWithDetailsAsync(deliveryId);
        return MapToResponse(updatedDelivery!);
    }

    private async Task ReclassifyDeliveriesOfDayAsync(int staffId, DateTime date)
    {
        var deliveries = await _deliveryRepo.GetByStaffAndDateRangeAsync(staffId, date, date);

        if (!deliveries.Any()) return;

        var sortedDeliveries = deliveries
            .Where(d => d.GroupSchedule?.SetupTime != null)
            .OrderBy(d => d.GroupSchedule!.SetupTime)
            .ToList();

        var count = sortedDeliveries.Count;

        for (int i = 0; i < count; i++)
        {
            var item = sortedDeliveries[i];
            DeliveryType newType;

            if (count == 1)
            {
                newType = DeliveryType.SoleDelivery;
            }
            else
            {
                if (i == 0) newType = DeliveryType.FirstOfDay;
                else if (i == count - 1) newType = DeliveryType.LastOfDay;
                else newType = DeliveryType.MidDay;
            }

            if (item.Type != newType)
            {
                item.Type = newType;
                await _deliveryRepo.UpdateAsync(item);
            }
        }
    }

    public async Task<ActualDeliveryResponse> UpdateStatusAsync(
        int deliveryId,
        UpdateDeliveryStatusRequest request,
        int staffId)
    {
        var delivery = await _deliveryRepo.GetWithDetailsAsync(deliveryId);

        if (delivery == null)
        {
            throw new Exception($"ActualDelivery {deliveryId} not found");
        }

        // Validate staff ownership
        if (delivery.StaffId != staffId)
        {
            throw new Exception("You can only update your own deliveries");
        }

        // Validate status transition
        var validTransitions = new Dictionary<string, string[]>
        {
            { "Pending", new[] { "Assigned" } },
            // Assigned có thể đi Dispatched (Rời kho) hoặc Delivering (Đi tiếp)
            { "Assigned", new[] { "Dispatched", "Delivering" } },
            // Dispatched (Rời kho) -> Giao xong luôn
            { "Dispatched", new[] { "Delivered" } },
            { "Delivering", new[] { "Delivered" } },
            // Delivered -> Returning (Về kho) hoặc kết thúc
            { "Delivered", new[] { "Returning" } }, 
            { "Returning", new[] { "Returned" } },
            { "Returned", new string[] { } }
        };

        if (!validTransitions.ContainsKey(delivery.Status))
        {
            throw new Exception($"Invalid current status: {delivery.Status}");
        }

        if (!validTransitions[delivery.Status].Contains(request.Status))
        {
            throw new Exception(
                $"Cannot transition from {delivery.Status} to {request.Status}. " +
                $"Valid transitions: {string.Join(", ", validTransitions[delivery.Status])}"
            );
        }

        // Validate Logic based on DeliveryType
        if (request.Status == "Dispatched")
        {
            // Only FirstOfDay or SoleDelivery can dispatch from warehouse
            if (delivery.Type != DeliveryType.FirstOfDay && delivery.Type != DeliveryType.SoleDelivery)
            {
                throw new Exception($"Delivery Type '{delivery.Type}' cannot start from warehouse (Dispatched). Only FirstOfDay or SoleDelivery allowed.");
            }
        }

        if (request.Status == "Returning" || request.Status == "Returned")
        {
             // Only LastOfDay or SoleDelivery can return to warehouse
             if (delivery.Type != DeliveryType.LastOfDay && delivery.Type != DeliveryType.SoleDelivery)
             {
                 throw new Exception($"Delivery Type '{delivery.Type}' cannot return to warehouse. Only LastOfDay or SoleDelivery allowed.");
             }
        }

        // Update status
        delivery.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Notes))
        {
            delivery.Notes = request.Notes;
        }

        // Set actual times
        if (request.Status == "Dispatched" || request.Status == "Delivering")
        {
            // Ghi nhận Giờ Xuất Phát (Departure Time)
            // Chỉ ghi nhận lần đầu tiên (nếu chưa có)
            if (!delivery.ActualDeliveryTime.HasValue)
            {
                delivery.ActualDeliveryTime = DateTime.UtcNow;
            }
        }
        else if (request.Status == "Delivered")
        {
            // KHÔNG ghi đè ActualDeliveryTime nữa.
            // ActualDeliveryTime sẽ giữ giá trị là "Giờ Xuất Phát".
            // "Giờ Đến" (Arrival) hiện tại sẽ không được lưu (theo yêu cầu).
        }
        else if (request.Status == "Returning")
        {
            // Returning = Bắt đầu về (Giờ thu hồi xong)
            delivery.ActualPickupTime = DateTime.UtcNow;
        }
        
        // Returned = Về đến kho -> Chỉ update UpdatedAt
        
        delivery.UpdatedAt = DateTime.UtcNow;

        await _deliveryRepo.UpdateAsync(delivery);

        return await MapToResponseAsync(delivery.Id);
    }

    public async Task<ActualDeliveryResponse> UpdateNotesAsync(
        int deliveryId,
        UpdateDeliveryNotesRequest request,
        int staffId)
    {
        var delivery = await _deliveryRepo.GetWithDetailsAsync(deliveryId);

        if (delivery == null)
        {
            throw new Exception($"ActualDelivery {deliveryId} not found");
        }

        if (delivery.StaffId != staffId)
        {
            throw new Exception("You can only update your own deliveries");
        }

        delivery.Notes = request.Notes;
        delivery.UpdatedAt = DateTime.UtcNow;

        await _deliveryRepo.UpdateAsync(delivery);

        return await MapToResponseAsync(delivery.Id);
    }

    public async Task<ActualDeliveryResponse> GetByIdAsync(int id)
    {
        var delivery = await _deliveryRepo.GetWithDetailsAsync(id);

        if (delivery == null)
        {
            throw new Exception($"ActualDelivery {id} not found");
        }

        return MapToResponse(delivery);
    }

    public async Task<ActualDeliveryResponse?> GetByGroupScheduleIdAsync(int groupScheduleId)
    {
        var delivery = await _deliveryRepo.GetByGroupScheduleIdAsync(groupScheduleId);

        return delivery == null ? null : MapToResponse(delivery);
    }

    public async Task<List<ActualDeliveryResponse>> GetByStaffIdAsync(int staffId)
    {
        var deliveries = await _deliveryRepo.GetByStaffIdAsync(staffId);

        return deliveries.Select(MapToResponse).ToList();
    }

    public async Task<List<DeliveryCalendarResponse>> GetCalendarAsync(
        DateTime from,
        DateTime to,
        int? staffId = null)
    {
        var deliveries = await _deliveryRepo.GetByDateRangeAsync(from, to);

        if (staffId.HasValue)
        {
            deliveries = deliveries.Where(d => d.StaffId == staffId.Value).ToList();
        }

        var grouped = deliveries
            .GroupBy(d => d.GroupSchedule.EventDate?.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DeliveryCalendarResponse
            {
                Date = g.Key?.ToString("yyyy-MM-dd") ?? "TBD",
                Deliveries = g.Select(MapToResponse).ToList(),
                TotalDeliveries = g.Count()
            })
            .ToList();

        return grouped;
    }

    public async Task<ConflictCheckResponse> CheckStaffConflictAsync(int staffId, int groupScheduleId)
    {
        // Get target schedule
        var targetSchedule = await _groupScheduleRepo.GetAsync(gs => gs.Id == groupScheduleId);
        if (targetSchedule == null)
        {
            throw new Exception($"GroupSchedule {groupScheduleId} not found");
        }

        if (!targetSchedule.EventDate.HasValue ||
            // !targetSchedule.DeliveryTime.HasValue || 
            !targetSchedule.FinishTime.HasValue)
        {
            throw new Exception("GroupSchedule must have EventDate, DeliveryTime, and FinishTime");
        }

        // Calculate target time range
        var targetStart = targetSchedule.EventDate.Value.Date;// + targetSchedule.DeliveryTime.Value.ToTimeSpan();
        var targetEnd = targetSchedule.EventDate.Value.Date + targetSchedule.FinishTime.Value.ToTimeSpan();

        // Get all deliveries of this staff around same date
        var staffDeliveries = await _deliveryRepo.GetByStaffAndDateRangeAsync(
            staffId,
            targetSchedule.EventDate.Value.AddDays(-1),
            targetSchedule.EventDate.Value.AddDays(1)
        );

        var conflicts = new List<ConflictDetail>();

        foreach (var delivery in staffDeliveries)
        {
            var schedule = delivery.GroupSchedule;

            if (!schedule.EventDate.HasValue ||
                // !schedule.DeliveryTime.HasValue || 
                !schedule.FinishTime.HasValue)
            {
                continue;
            }

            var existStart = schedule.EventDate.Value.Date;// + schedule.DeliveryTime.Value.ToTimeSpan();
            var existEnd = schedule.EventDate.Value.Date + schedule.FinishTime.Value.ToTimeSpan();

            // Check overlap
            if (targetStart < existEnd && targetEnd > existStart)
            {
                conflicts.Add(new ConflictDetail
                {
                    DeliveryId = delivery.Id,
                    EventName = schedule.Rental?.EventName ?? "Unknown Event",
                    ScheduledStart = existStart,
                    ScheduledEnd = existEnd
                });
            }
        }

        return new ConflictCheckResponse
        {
            HasConflict = conflicts.Any(),
            Conflicts = conflicts
        };
    }

    public async Task<PageListResponse<ActualDeliveryResponse>> GetPendingDeliveriesAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? sortBy = "date")
    {
        // Validate parameters
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 50;

        // Get from repository
        var (items, totalCount) = await _deliveryRepo.GetPendingDeliveriesAsync(
            page,
            pageSize,
            searchTerm,
            sortBy
        );

        // Map to response DTOs
        var responseDtos = items.Select(MapToResponse).ToList();

        return new PageListResponse<ActualDeliveryResponse>
        {
            Items = responseDtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
    }

    // Helper methods
    private async Task<ActualDeliveryResponse> MapToResponseAsync(int deliveryId)
    {
        var delivery = await _deliveryRepo.GetWithDetailsAsync(deliveryId);
        return MapToResponse(delivery!);
    }

    private ActualDeliveryResponse MapToResponse(ActualDelivery delivery)
    {
        var schedule = delivery.GroupSchedule;
        var rental = schedule?.Rental;

        DateTime? scheduledDeliveryTime = null;
        DateTime? scheduledPickupTime = null;

        if (schedule?.EventDate.HasValue == true)
        {
            // Calculate delivery time: SetupTime - TravelTime
            if (schedule.SetupTime.HasValue && !string.IsNullOrWhiteSpace(schedule.EventCity))
            {
                var (_, distance) = DeliveryFeeConfig.CalculateFee(schedule.EventCity);
                var travelTimeHours = DeliveryFeeConfig.GetTravelTimeHours(distance);
                
                scheduledDeliveryTime = schedule.EventDate.Value.Date 
                    + schedule.SetupTime.Value.ToTimeSpan() 
                    - TimeSpan.FromHours(travelTimeHours);
            }
            
            if (schedule.FinishTime.HasValue)
            {
                scheduledPickupTime = schedule.EventDate.Value.Date + schedule.FinishTime.Value.ToTimeSpan();
            }
        }

        return new ActualDeliveryResponse
        {
            Id = delivery.Id,
            GroupScheduleId = delivery.GroupScheduleId,
            StaffId = delivery.StaffId,
            StaffName = delivery.Staff?.FullName,
            Type = delivery.Type,

            ScheduledDeliveryTime = scheduledDeliveryTime,
            ScheduledPickupTime = scheduledPickupTime,

            ActualDeliveryTime = delivery.ActualDeliveryTime,
            ActualPickupTime = delivery.ActualPickupTime,

            Status = delivery.Status,
            Notes = delivery.Notes,

            CreatedAt = delivery.CreatedAt,
            UpdatedAt = delivery.UpdatedAt,

            ScheduleInfo = schedule == null ? null : new GroupScheduleInfo
            {
                EventDate = schedule.EventDate,
                EventLocation = schedule.EventLocation,
                EventCity = schedule.EventCity,
                DeliveryTime = CalculateDeliveryTime(schedule),
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                FinishTime = schedule.FinishTime
            },

            RentalInfo = rental == null ? null : new RentalInfo
            {
                RentalId = rental.Id,
                EventName = rental.EventName,
                CustomerName = rental.Account?.FullName,
                PhoneNumber = rental.PhoneNumber,
                PackageName = rental.ActivityType?.Name // Map Package Name
            }
        };
    }

    private TimeOnly? CalculateDeliveryTime(GroupSchedule schedule)
    {
        if (!schedule.SetupTime.HasValue || string.IsNullOrWhiteSpace(schedule.EventCity))
            return null;

        var (_, distance) = DeliveryFeeConfig.CalculateFee(schedule.EventCity);
        var travelTimeHours = DeliveryFeeConfig.GetTravelTimeHours(distance);
        
        var setupDateTime = DateTime.Today + schedule.SetupTime.Value.ToTimeSpan();
        var deliveryDateTime = setupDateTime - TimeSpan.FromHours(travelTimeHours);
        
        return TimeOnly.FromDateTime(deliveryDateTime);
    }
    public async Task<ActualDeliveryResponse?> GetByRentalIdAsync(int rentalId)
    {
        // Query: Rental → GroupSchedules → ActualDelivery (first one)
        var groupSchedules = await _groupScheduleRepo.GetAllAsync(
            gs => gs.RentalId == rentalId,
            includeProperties: "Rental,Rental.Account,Rental.ActivityType"
        );

        if (!groupSchedules.Any())
        {
            return null;
        }

        // Get first group schedule with delivery
        foreach (var schedule in groupSchedules.OrderBy(s => s.EventDate))
        {
            var delivery = await _deliveryRepo.GetByGroupScheduleIdAsync(schedule.Id);
            if (delivery != null)
            {
                return MapToResponse(delivery);
            }
        }

        return null;
    }

    public async Task<AssignStaffBatchResponse> AssignStaffBatchAsync(AssignStaffBatchRequest request)
    {
        var existingAssignments = await _deliveryRepo.GetByStaffAndDateRangeAsync(
            request.StaffId,
            request.EventDate,
            request.EventDate
        );

        if (existingAssignments.Any())
        {
            var assignedGroupIds = existingAssignments
                .Select(d => d.GroupSchedule.ActivityTypeGroupId)
                .Distinct()
                .ToList();

            if (!assignedGroupIds.Contains(request.ActivityTypeGroupId) || assignedGroupIds.Count > 1)
            {
                throw new Exception(
                    $"Staff đã được assign cho ActivityTypeGroup khác trong ngày {request.EventDate:yyyy-MM-dd}. " +
                    $"Không thể assign cho nhiều nhóm robot trong cùng một ngày.");
            }
        }

        var groupSchedules = await _groupScheduleRepo.GetByEventDateAndActivityTypeGroupAsync(
            request.EventDate,
            request.ActivityTypeGroupId
        );

        if (!groupSchedules.Any())
        {
            throw new Exception(
                $"Không tìm thấy GroupSchedule nào cho (Date: {request.EventDate:yyyy-MM-dd}, ActivityTypeGroup: {request.ActivityTypeGroupId})");
        }

        var sortedSchedules = groupSchedules.OrderBy(gs => gs.SetupTime).ToList();
        var conflictingScheduleIds = new List<int>();
        var assignableScheduleIds = new List<int>();

        // Check travel time conflicts
        for (int i = 0; i < sortedSchedules.Count; i++)
        {
            var current = sortedSchedules[i];
            
            // First schedule is always assignable
            if (i == 0)
            {
                assignableScheduleIds.Add(current.Id);
                continue;
            }

            var previous = sortedSchedules[i - 1];

            if (current.SetupTime == null || current.FinishTime == null ||
                previous.SetupTime == null || previous.FinishTime == null)
            {
                throw new Exception($"Schedule {current.Id} hoặc {previous.Id} thiếu thời gian.");
            }

            var finishDateTime = previous.EventDate!.Value.Date + previous.FinishTime.Value.ToTimeSpan();
            var nextSetupDateTime = current.EventDate!.Value.Date + current.SetupTime.Value.ToTimeSpan();

            var (_, distance) = DeliveryFeeConfig.CalculateFee(current.EventCity);
            var travelHours = DeliveryFeeConfig.GetTravelTimeHours(distance);

            var requiredArrival = finishDateTime.AddHours(travelHours);

            if (requiredArrival > nextSetupDateTime)
            {
                // Conflict detected - all remaining schedules are conflicting
                for (int j = i; j < sortedSchedules.Count; j++)
                {
                    conflictingScheduleIds.Add(sortedSchedules[j].Id);
                }
                break;
            }
            else
            {
                assignableScheduleIds.Add(current.Id);
            }
        }

        // If conflict detected and not forcing partial assign, return conflict info
        if (conflictingScheduleIds.Any() && !request.ForcePartialAssign)
        {
            var conflictMessages = new List<string>();
            for (int i = 0; i < sortedSchedules.Count - 1; i++)
            {
                var current = sortedSchedules[i];
                var next = sortedSchedules[i + 1];
                
                if (conflictingScheduleIds.Contains(next.Id))
                {
                    var finishDateTime = current.EventDate!.Value.Date + current.FinishTime!.Value.ToTimeSpan();
                    var nextSetupDateTime = next.EventDate!.Value.Date + next.SetupTime!.Value.ToTimeSpan();
                    var gap = (nextSetupDateTime - finishDateTime).TotalHours;
                    var (_, distance) = DeliveryFeeConfig.CalculateFee(next.EventCity);
                    var travelHours = DeliveryFeeConfig.GetTravelTimeHours(distance);
                    
                    conflictMessages.Add(
                        $"{current.Rental?.EventName ?? current.RentalId.ToString()} kết thúc lúc {current.FinishTime}, " +
                        $"{next.Rental?.EventName ?? next.RentalId.ToString()} bắt đầu lúc {next.SetupTime}. " +
                        $"Cần {travelHours}h di chuyển đến {next.EventCity} nhưng chỉ còn {gap:F1}h."
                    );
                    break; // Only show first conflict
                }
            }

            return new AssignStaffBatchResponse
            {
                Success = false,
                HasConflict = true,
                ConflictingScheduleIds = conflictingScheduleIds,
                AssignedScheduleIds = new List<int>(),
                AssignedCount = 0,
                ConflictMessage = $"Staff không thể hoàn thành tất cả {sortedSchedules.Count} lịch trình. " +
                                 $"Chỉ có thể assign {assignableScheduleIds.Count} lịch trình đầu tiên. " +
                                 string.Join(" ", conflictMessages)
            };
        }

        // Get schedules to assign (only assignable ones if there's conflict)
        var schedulesToAssign = conflictingScheduleIds.Any() 
            ? sortedSchedules.Where(gs => assignableScheduleIds.Contains(gs.Id)).ToList()
            : sortedSchedules;

        var pendingDeliveries = new List<ActualDelivery>();
        foreach (var gs in schedulesToAssign)
        {
            var delivery = await _deliveryRepo.GetByGroupScheduleIdAsync(gs.Id);
            if (delivery != null && delivery.Status == "Pending")
            {
                pendingDeliveries.Add(delivery);
            }
        }

        if (!pendingDeliveries.Any())
        {
            throw new Exception($"Không có pending delivery nào để assign.");
        }

        var assignedIds = new List<int>();
        foreach (var delivery in pendingDeliveries)
        {
            delivery.StaffId = request.StaffId;
            delivery.Status = "Assigned";
            delivery.Notes = request.Notes;
            delivery.UpdatedAt = DateTime.UtcNow;

            await _deliveryRepo.UpdateAsync(delivery);
            assignedIds.Add(delivery.GroupScheduleId);
        }

        await ReclassifyDeliveriesOfDayAsync(request.StaffId, request.EventDate);

        return new AssignStaffBatchResponse
        {
            Success = true,
            AssignedCount = pendingDeliveries.Count,
            HasConflict = conflictingScheduleIds.Any(),
            ConflictingScheduleIds = conflictingScheduleIds,
            AssignedScheduleIds = assignedIds,
            ConflictMessage = conflictingScheduleIds.Any() 
                ? $"Đã assign {pendingDeliveries.Count}/{sortedSchedules.Count} lịch trình do conflict."
                : null
        };
    }

    public async Task<PendingDeliveriesGroupedResponse> GetPendingDeliveriesGroupedAsync(
        DateTime? from = null,
        DateTime? to = null)
    {
        var defaultFrom = DateTime.UtcNow.Date;
        var defaultTo = DateTime.UtcNow.Date.AddDays(60);

        from = from ?? defaultFrom;
        to = to ?? defaultTo;

        // CHỈ lấy deliveries có status = Pending
        var allDeliveries = await _deliveryRepo.GetByDateRangeAsync(from.Value, to.Value);
        var pendingDeliveries = allDeliveries
            .Where(d => d.Status == "Pending")
            .Where(d => d.GroupSchedule?.EventDate.HasValue == true && d.GroupSchedule.ActivityTypeGroupId.HasValue)
            .ToList();

        // Group by EventDate + ActivityTypeGroupId
        var grouped = pendingDeliveries
            .GroupBy(d => new
            {
                Date = d.GroupSchedule!.EventDate!.Value.Date,
                GroupId = d.GroupSchedule.ActivityTypeGroupId!.Value
            })
            .Select(g =>
            {
                var schedules = g.Select(d => d.GroupSchedule!).Distinct().OrderBy(gs => gs.SetupTime).ToList();
                var firstSchedule = schedules.First();
                var activityTypeGroup = firstSchedule.ActivityTypeGroup;
                var activityType = activityTypeGroup?.ActivityType;
                
                // Tên group = ActivityType Name (ví dụ: "Wedding Robots", "Corporate Event Robots")
                var groupName = $"{activityType?.Name ?? "Unknown"} ATG {g.Key.GroupId}";

                return new GroupedDeliveryInfo
                {
                    ActivityTypeGroupId = g.Key.GroupId,
                    ActivityTypeGroupName = groupName,
                    EventDate = g.Key.Date.ToString("yyyy-MM-dd"),
                    DeliveryIds = g.Select(d => d.Id).ToList(),
                    GroupScheduleIds = schedules.Select(gs => gs.Id).ToList(),
                    ScheduleCount = schedules.Count,
                    ScheduleInfo = new ScheduleInfoSummary
                    {
                        EventLocations = schedules.Select(gs => gs.EventLocation ?? "").Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList(),
                        EventCities = schedules.Select(gs => gs.EventCity ?? "").Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList(),
                        EarliestSetupTime = schedules.First().SetupTime?.ToString() ?? "",
                        LatestFinishTime = schedules.Last().FinishTime?.ToString() ?? ""
                    },
                    RentalInfo = new RentalInfoSummary
                    {
                        EventNames = schedules.Select(gs => gs.Rental?.EventName ?? "").Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList(),
                        CustomerNames = schedules.Select(gs => gs.Rental?.Account?.FullName ?? "").Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList()
                    }
                };
            })
            .OrderBy(g => g.EventDate)
            .ThenBy(g => g.ActivityTypeGroupName)
            .ToList();

        return new PendingDeliveriesGroupedResponse
        {
            Groups = grouped,
            From = from.Value.ToString("yyyy-MM-dd"),
            To = to.Value.ToString("yyyy-MM-dd")
        };
    }



}