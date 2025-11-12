using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOs.ActivityTypeGroup;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Repository.Repositories;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ActivityTypeGroupService : IActivityTypeGroupService
{
    private readonly IActivityTypeGroupRepository _repository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IMapper _mapper;
    
    public ActivityTypeGroupService(IActivityTypeGroupRepository repository, IMapper mapper,  IRentalRepository rentalRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _rentalRepository = rentalRepository;
    }

    public async Task<List<ActivityTypeGroupResponse>> GetAllActivityTypeGroupsAsync()
    {
        var result = await _repository.GetDbContext().ActivityTypeGroups
            .Include(a => a.ActivityType)
            .ThenInclude(a => a.EventActivity)
            .ToListAsync();

        return result.Select(atg => _mapper.Map<ActivityTypeGroupResponse>(atg)).ToList();
    }

    public async Task<List<ActivityTypeGroupResponse>> GetAllActivityTypeGroupsSuitableForRentalAsync(int rentalId)
    {
        // 1️⃣ Get rental info
        var rental = await _rentalRepository.GetAsync(r => r.Id == rentalId);
        if (rental == null)
            return null;

        // 2️⃣ Get groups with their schedules
        var groups = await _repository.GetAllAsync(
            a => a.ActivityTypeId == rental.ActivityTypeId,
            includeProperties: "GroupSchedules"
        );

        // 3️⃣ Separate groups that match the schedule overlap condition
        var matchedGroups = new List<ActivityTypeGroup>();
        var otherGroups = new List<ActivityTypeGroup>();

        foreach (var group in groups)
        {
            bool matched = group.GroupSchedules.Any(schedule =>
                schedule.EventDate == rental.EventDate &&
                schedule.EventCity == rental.City &&
                (
                    (rental.EndTime <= schedule.DeliveryTime) || (rental.StartTime >= schedule.FinishTime)
                )
            );

            if (matched)
                matchedGroups.Add(group);
            else
                otherGroups.Add(group);
        }

        // 4️⃣ Merge lists: matched ones first
        var orderedGroups = matchedGroups.Concat(otherGroups).ToList();

        // 5️⃣ Map to response DTOs
        return orderedGroups.Select(g => _mapper.Map<ActivityTypeGroupResponse>(g)).ToList();
    }

}