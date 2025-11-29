using RoboRent_BE.Model.DTOs.ActualDelivery;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ActualDeliveryService : IActualDeliveryService
{
    private readonly IActualDeliveryRepository _deliveryRepo;
    private readonly IRentalRepository _rentalRepo;

    public ActualDeliveryService(
        IActualDeliveryRepository deliveryRepo,
        IRentalRepository rentalRepo)
    {
        _deliveryRepo = deliveryRepo;
        _rentalRepo = rentalRepo;
    }

    public async Task<ActualDeliveryResponse> CreateActualDeliveryAsync(CreateActualDeliveryRequest request)
    {
        // Validate rental exists and status = AcceptedContract
        var rental = await _rentalRepo.GetAsync(r => r.Id == request.RentalId);
    
        if (rental == null)
        {
            throw new Exception($"Rental {request.RentalId} not found");
        }

        if (rental.Status != "AcceptedContract")
        {
            throw new Exception($"Cannot create delivery. Rental status: {rental.Status}. Required: AcceptedContract");
        }

        // Check if delivery already exists
        var existingDelivery = await _deliveryRepo.GetByRentalIdAsync(request.RentalId);
        if (existingDelivery != null)
        {
            throw new Exception($"Delivery already exists for rental {request.RentalId}");
        }

        // ✅ NEW: Auto assign cho Rental.StaffId và dùng PlannedTimes
        var delivery = new ActualDelivery
        {
            RentalId = request.RentalId,
            StaffId = rental.StaffId, // ✅ Auto assign
            ScheduledDeliveryTime = rental.PlannedDeliveryTime, // ✅ Dùng planned
            ScheduledPickupTime = rental.PlannedPickupTime, // ✅ Dùng planned
            Status = "Assigned", // ✅ Đổi luôn thành Assigned (đã có staff)
            CustomerRequestNotes = request.CustomerRequestNotes,
            CreatedAt = DateTime.UtcNow
        };

        await _deliveryRepo.AddAsync(delivery);

        return await MapToResponseAsync(delivery);
    }

    public async Task<ActualDeliveryResponse> AssignDeliveryAsync(int deliveryId, AssignDeliveryRequest request, int staffId)
    {
        var delivery = await _deliveryRepo.GetAsync(d => d.Id == deliveryId);
    
        if (delivery == null)
        {
            throw new Exception("Delivery not found");
        }

        // ✅ NEW: Cho phép staff edit scheduled times (status vẫn là Assigned)
        if (delivery.Status != "Assigned")
        {
            throw new Exception($"Cannot update delivery with status: {delivery.Status}. Required: Assigned");
        }

        // ✅ NEW: Chỉ staff owner mới edit được
        if (delivery.StaffId != staffId)
        {
            throw new Exception("You can only update your own deliveries");
        }

        var rental = await _rentalRepo.GetAsync(r => r.Id == delivery.RentalId);

        // Staff có thể override scheduled times hoặc giữ nguyên
        var scheduledDeliveryTime = request.ScheduledDeliveryTime ?? delivery.ScheduledDeliveryTime;
        var scheduledPickupTime = request.ScheduledPickupTime ?? delivery.ScheduledPickupTime;

        // Validate times nếu có EventDate
        if (rental.EventDate.HasValue && scheduledDeliveryTime.HasValue)
        {
            if (scheduledDeliveryTime.Value >= rental.EventDate.Value.AddHours(-2))
            {
                throw new Exception("ScheduledDeliveryTime must be at least 2 hours before EventDate");
            }
        }

        if (rental.EventDate.HasValue && scheduledPickupTime.HasValue)
        {
            if (scheduledPickupTime.Value <= rental.EventDate.Value)
            {
                throw new Exception("ScheduledPickupTime must be after EventDate");
            }
        }

        // Update times (staff điều chỉnh lịch)
        delivery.ScheduledDeliveryTime = scheduledDeliveryTime;
        delivery.ScheduledPickupTime = scheduledPickupTime;
        if (!string.IsNullOrEmpty(request.Notes))
        {
            delivery.Notes = request.Notes;
        }

        await _deliveryRepo.UpdateAsync(delivery);

        return await MapToResponseAsync(delivery);
    }

    public async Task<ActualDeliveryResponse> UpdateStatusAsync(int deliveryId, UpdateDeliveryStatusRequest request, int staffId)
    {
        var delivery = await _deliveryRepo.GetAsync(d => d.Id == deliveryId);
        
        if (delivery == null)
        {
            throw new Exception("Delivery not found");
        }

        if (delivery.StaffId != staffId)
        {
            throw new Exception("You can only update your own deliveries");
        }

        // Validate status transition
        var validTransitions = new Dictionary<string, string[]>
        {
            { "Planning", new[] { "Assigned" } },
            { "Assigned", new[] { "Delivering" } },
            { "Delivering", new[] { "Delivered" } },
            { "Delivered", new[] { "Collecting" } },
            { "Collecting", new[] { "Collected" } },
            { "Collected", new[] { "Completed" } }
        };

        if (!validTransitions.ContainsKey(delivery.Status))
        {
            throw new Exception($"Invalid current status: {delivery.Status}");
        }

        if (!validTransitions[delivery.Status].Contains(request.Status))
        {
            throw new Exception($"Cannot transition from {delivery.Status} to {request.Status}");
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

        await _deliveryRepo.UpdateAsync(delivery);

        return await MapToResponseAsync(delivery);
    }

    public async Task<ActualDeliveryResponse> GetByIdAsync(int id)
    {
        var delivery = await _deliveryRepo.GetAsync(
            d => d.Id == id,
            includeProperties: "Rental,Rental.Account,Staff"
        );
        
        if (delivery == null)
        {
            throw new Exception("Delivery not found");
        }

        return MapToResponse(delivery);
    }

    public async Task<ActualDeliveryResponse> GetByRentalIdAsync(int rentalId)
    {
        var delivery = await _deliveryRepo.GetByRentalIdAsync(rentalId);
        
        if (delivery == null)
        {
            throw new Exception($"No delivery found for rental {rentalId}");
        }

        return MapToResponse(delivery);
    }

    public async Task<List<ActualDeliveryResponse>> GetByStaffIdAsync(int staffId)
    {
        var deliveries = await _deliveryRepo.GetByStaffIdAsync(staffId);
        
        return deliveries.Select(MapToResponse).ToList();
    }

    public async Task<List<DeliveryCalendarResponse>> GetCalendarAsync(DateTime from, DateTime to, int? staffId = null)
    {
        var deliveries = await _deliveryRepo.GetByDateRangeAsync(from, to);

        if (staffId.HasValue)
        {
            deliveries = deliveries.Where(d => d.StaffId == staffId.Value).ToList();
        }

        var grouped = deliveries
            .GroupBy(d => d.ScheduledDeliveryTime?.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DeliveryCalendarResponse
            {
                Date = g.Key?.ToString("MMM dd, yyyy") ?? "TBD",
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

    // Helper methods
    private async Task<ActualDeliveryResponse> MapToResponseAsync(ActualDelivery delivery)
    {
        var fullDelivery = await _deliveryRepo.GetAsync(
            d => d.Id == delivery.Id,
            includeProperties: "Rental,Rental.Account,Staff"
        );
        
        return MapToResponse(fullDelivery);
    }

    private ActualDeliveryResponse MapToResponse(ActualDelivery delivery)
    {
        return new ActualDeliveryResponse
        {
            Id = delivery.Id,
            RentalId = delivery.RentalId,
            RentalEventName = delivery.Rental?.EventName ?? "Unknown",
            CustomerName = delivery.Rental?.Account?.FullName ?? "Unknown",
            StaffId = delivery.StaffId,
            StaffName = delivery.Staff?.FullName,
            ScheduledDeliveryTime = delivery.ScheduledDeliveryTime,
            ActualDeliveryTime = delivery.ActualDeliveryTime,
            ScheduledPickupTime = delivery.ScheduledPickupTime,
            ActualPickupTime = delivery.ActualPickupTime,
            Status = delivery.Status,
            CustomerRequestNotes = delivery.CustomerRequestNotes,
            Notes = delivery.Notes,
            CreatedAt = delivery.CreatedAt,
            UpdatedAt = delivery.UpdatedAt
        };
    }
}