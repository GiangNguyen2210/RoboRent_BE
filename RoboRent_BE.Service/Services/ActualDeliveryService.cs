using RoboRent_BE.Model.DTOs;
using RoboRent_BE.Model.DTOs.ActualDelivery;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
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
            includeProperties: "Rental,Rental.Account"
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

        // Assign staff
        delivery.StaffId = request.StaffId;
        delivery.Status = "Assigned";
        delivery.Notes = request.Notes;
        delivery.UpdatedAt = DateTime.UtcNow;

        await _deliveryRepo.UpdateAsync(delivery);

        return await MapToResponseAsync(delivery.Id);
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
            { "Assigned", new[] { "Delivering" } },
            { "Delivering", new[] { "Delivered" } },
            { "Delivered", new string[] { } } 
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

        // Update status
        delivery.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Notes))
        {
            delivery.Notes = request.Notes;
        }

        // Set actual times
        if (request.Status == "Delivered")
        {
            delivery.ActualDeliveryTime = DateTime.UtcNow;
        }
        else if (request.Status == "Collected")
        {
            delivery.ActualPickupTime = DateTime.UtcNow;
        }

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
            !targetSchedule.DeliveryTime.HasValue || 
            !targetSchedule.FinishTime.HasValue)
        {
            throw new Exception("GroupSchedule must have EventDate, DeliveryTime, and FinishTime");
        }

        // Calculate target time range
        var targetStart = targetSchedule.EventDate.Value.Date + targetSchedule.DeliveryTime.Value.ToTimeSpan();
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
                !schedule.DeliveryTime.HasValue || 
                !schedule.FinishTime.HasValue)
            {
                continue;
            }

            var existStart = schedule.EventDate.Value.Date + schedule.DeliveryTime.Value.ToTimeSpan();
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
            if (schedule.DeliveryTime.HasValue)
            {
                scheduledDeliveryTime = schedule.EventDate.Value.Date + schedule.DeliveryTime.Value.ToTimeSpan();
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
                DeliveryTime = schedule.DeliveryTime,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                FinishTime = schedule.FinishTime
            },
            
            RentalInfo = rental == null ? null : new RentalInfo
            {
                RentalId = rental.Id,
                EventName = rental.EventName,
                CustomerName = rental.Account?.FullName,
                PhoneNumber = rental.PhoneNumber
            }
        };
    }
    public async Task<ActualDeliveryResponse?> GetByRentalIdAsync(int rentalId)
    {
        // Query: Rental → GroupSchedules → ActualDelivery (first one)
        var groupSchedules = await _groupScheduleRepo.GetAllAsync(
            gs => gs.RentalId == rentalId,
            includeProperties: "Rental,Rental.Account"
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
    
    
}