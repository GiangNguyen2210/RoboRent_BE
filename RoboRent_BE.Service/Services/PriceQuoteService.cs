using RoboRent_BE.Model.DTOs.PriceQuote;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Configuration;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class PriceQuoteService : IPriceQuoteService
{
    private readonly IPriceQuoteRepository _priceQuoteRepo;
    private readonly IRentalRepository _rentalRepo;
    private readonly IGroupScheduleRepository _groupScheduleRepo;
    private readonly IRentalDetailRepository _rentalDetailRepo;
    private readonly IRobotAbilityValueRepository _robotAbilityValueRepo;
    
    public PriceQuoteService(
        IPriceQuoteRepository priceQuoteRepo,
        IRentalRepository rentalRepo,
        IGroupScheduleRepository groupScheduleRepo,
        IRentalDetailRepository rentalDetailRepo,
        IRobotAbilityValueRepository robotAbilityValueRepo)
    {
        _priceQuoteRepo = priceQuoteRepo;
        _rentalRepo = rentalRepo;
        _groupScheduleRepo = groupScheduleRepo;
        _rentalDetailRepo = rentalDetailRepo;
        _robotAbilityValueRepo = robotAbilityValueRepo;
    }

    public async Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId)
    {
        // Validate rental status FIRST
        var rental = await _rentalRepo.GetAsync(r => r.Id == request.RentalId, "ActivityType");
        if (rental == null)
            throw new Exception("Rental not found");

        var validStatuses = new[] { "Received", "PendingPriceQuote", "RejectedPriceQuote" };
        if (!validStatuses.Contains(rental.Status))
        {
            throw new Exception($"Không thể tạo quote. Rental status hiện tại: {rental.Status}. Cần status: Received hoặc PendingPriceQuote");
        }

        // Business Rule: Max 3 quotes per rental -> REMOVED
        var existingQuotes = await _priceQuoteRepo.GetByRentalIdAsync(request.RentalId);

        // if (existingQuotes.Count >= 3)
        // {
        //     throw new Exception($"Maximum 3 quotes reached for rental {request.RentalId}. Cannot create more.");
        // }

        // Check không có quote nào đang active
        var activeQuote = existingQuotes.FirstOrDefault(q =>
            q.Status == "PendingManager" ||
            q.Status == "PendingCustomer" ||
            q.Status == "Approved");

        if (activeQuote != null)
        {
            throw new Exception($"Cannot create new quote. Active quote exists with status: {activeQuote.Status}");
        }

        // === AUTO-CALCULATE DEPOSIT COMPONENTS ===
        var activityType = rental.ActivityType;
        if (activityType == null)
            throw new Exception("ActivityType not found for this rental");

        var calculatedFees = CalculateQuoteFeesFromRental(rental, activityType);

        // Create new quote with auto-calculated values
        var quote = new PriceQuote
        {
            RentalId = request.RentalId,
            // LOCKED deposit components (auto-calculated)
            RentalFee = calculatedFees.RentalFee,
            StaffFee = calculatedFees.StaffFee,
            DamageDeposit = calculatedFees.DamageDeposit,
            // Adjustable fees
            DeliveryFee = calculatedFees.DeliveryFee,
            DeliveryDistance = calculatedFees.DeliveryDistance,
            CustomizationFee = request.CustomizationFee ?? 0m,
            // Metadata
            StaffDescription = request.StaffDescription,
            CreatedAt = DateTime.UtcNow,
            Status = "PendingManager",
            SubmittedToManagerAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _priceQuoteRepo.AddAsync(quote);

        rental.Status = "PendingPriceQuote";
        await _rentalRepo.UpdateAsync(rental);

        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        return MapToPriceQuoteResponse(quote, allQuotes.Count);
    }

    public async Task<PriceQuoteResponse> GetPriceQuoteAsync(int id)
    {
        var quote = await _priceQuoteRepo.GetAsync(pq => pq.Id == id && pq.IsDeleted != true);

        if (quote == null)
        {
            throw new Exception($"Price quote not found");
        }

        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        var quoteNumber = allQuotes.OrderBy(q => q.CreatedAt).ToList().FindIndex(q => q.Id == id) + 1;

        return MapToPriceQuoteResponse(quote, quoteNumber);
    }

    public async Task<RentalQuotesResponse> GetQuotesByRentalIdAsync(int rentalId)
    {
        var quotes = await _priceQuoteRepo.GetByRentalIdAsync(rentalId);

        var quoteResponses = quotes
            .Select((q, index) => MapToPriceQuoteResponse(q, index + 1))
            .ToList();

        // ✅ CanCreateMore: count < 3 VÀ không có quote active -> REMOVED LIMIT
        var hasActiveQuote = quotes.Any(q =>
            q.Status == "PendingManager" ||
            q.Status == "PendingCustomer" ||
            q.Status == "Approved");

        return new RentalQuotesResponse
        {
            RentalId = rentalId,
            Quotes = quoteResponses,
            TotalQuotes = quotes.Count,
            CanCreateMore = !hasActiveQuote
        };
    }

    public async Task<PriceQuoteResponse> ManagerActionAsync(int quoteId, ManagerActionRequest request, int managerId)
    {
        var quote = await _priceQuoteRepo.GetAsync(q => q.Id == quoteId && q.IsDeleted != true);

        if (quote == null) throw new Exception("Quote not found");
        if (quote.Status != "PendingManager")
            throw new Exception($"Cannot perform action on quote with status: {quote.Status}");

        quote.ManagerId = managerId;

        if (request.Action.ToLower() == "approve")
        {
            quote.Status = "PendingCustomer";
            quote.ManagerApprovedAt = DateTime.UtcNow;
        }
        else if (request.Action.ToLower() == "reject")
        {
            if (string.IsNullOrWhiteSpace(request.Feedback))
            {
                throw new Exception("Feedback is required when rejecting");
            }

            quote.Status = "RejectedManager";
            quote.ManagerFeedback = request.Feedback;
        }
        else
        {
            throw new Exception("Invalid action. Use 'approve' or 'reject'");
        }

        await _priceQuoteRepo.UpdateAsync(quote);

        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        var quoteNumber = allQuotes.OrderBy(q => q.CreatedAt).ToList().FindIndex(q => q.Id == quoteId) + 1;

        return MapToPriceQuoteResponse(quote, quoteNumber);
    }

    public async Task<PriceQuoteResponse> CustomerActionAsync(int quoteId, CustomerActionRequest request, int customerId)
    {
        var quote = await _priceQuoteRepo.GetAsync(q => q.Id == quoteId && q.IsDeleted != true);

        if (quote == null) throw new Exception("Quote not found");
        if (quote.Status != "PendingCustomer")
            throw new Exception($"Cannot perform action on quote with status: {quote.Status}");

        if (request.Action.ToLower() == "approve")
        {
            // ✅ Accept this quote
            quote.Status = "Approved";
            await _priceQuoteRepo.UpdateAsync(quote);

            var rental = await _rentalRepo.GetAsync(r => r.Id == quote.RentalId);
            rental.Status = "AcceptedPriceQuote";
            await _rentalRepo.UpdateAsync(rental);

            // ✅ Auto reject other pending quotes
            var otherPendingQuotes = await _priceQuoteRepo
                .GetAllAsync(q =>
                    q.RentalId == quote.RentalId &&
                    q.Id != quoteId &&
                    q.Status == "PendingCustomer" &&
                    q.IsDeleted != true);

            foreach (var otherQuote in otherPendingQuotes)
            {
                otherQuote.Status = "RejectedCustomer";
                await _priceQuoteRepo.UpdateAsync(otherQuote);
            }
        }
        else if (request.Action.ToLower() == "reject")
        {
            // Check xem đã có bao nhiêu quotes
            var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);

            var rental2 = await _rentalRepo.GetAsync(r => r.Id == quote.RentalId);
            rental2.Status = "RejectedPriceQuote";
            await _rentalRepo.UpdateAsync(rental2);

            // REMOVED LIMIT CHECK (Previously >= 3 check)
            // Always allow creating new quote (RejectedCustomer status)
            
            quote.Status = "RejectedCustomer";
            await _priceQuoteRepo.UpdateAsync(quote);
            
            // The expired logic is removed as per new business rule (unlimited quotes)
        }
        else
        {
            throw new Exception("Invalid action. Use 'approve' or 'reject'");
        }

        var allQuotesForNumber = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        var quoteNumber = allQuotesForNumber.OrderBy(q => q.CreatedAt).ToList().FindIndex(q => q.Id == quoteId) + 1;

        return MapToPriceQuoteResponse(quote, quoteNumber);
    }

    public async Task<PriceQuoteResponse> UpdatePriceQuoteAsync(int quoteId, UpdatePriceQuoteRequest request, int staffId)
    {
        var quote = await _priceQuoteRepo.GetAsync(q => q.Id == quoteId && q.IsDeleted != true);

        if (quote == null) throw new Exception("Quote not found");

        // Chỉ cho phép update khi status = RejectedManager hoặc PendingManager
        if (quote.Status != "PendingManager" && quote.Status != "RejectedManager")
        {
            throw new Exception($"Cannot update quote with status: {quote.Status}. Only RejectedManager quotes can be updated.");
        }

        // === PHASE 2: Only allow updating adjustable fields ===
        // RentalFee, StaffFee, DamageDeposit are LOCKED
        if (request.DeliveryFee.HasValue) quote.DeliveryFee = request.DeliveryFee.Value;
        if (request.CustomizationFee.HasValue) quote.CustomizationFee = request.CustomizationFee.Value;
        if (request.StaffDescription != null) quote.StaffDescription = request.StaffDescription;

        // Auto submit lại to Manager
        quote.Status = "PendingManager";
        quote.SubmittedToManagerAt = DateTime.UtcNow;

        await _priceQuoteRepo.UpdateAsync(quote);

        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        var quoteNumber = allQuotes.OrderBy(q => q.CreatedAt).ToList().FindIndex(q => q.Id == quoteId) + 1;

        return MapToPriceQuoteResponse(quote, quoteNumber);
    }

    public async Task<List<ManagerQuoteListItemResponse>> GetAllQuotesForManagerAsync(string? status = null)
    {
        var quotes = await _priceQuoteRepo.GetAllWithDetailsAsync(status);

        return quotes.Select(pq =>
        {
            var allRentalQuotes = _priceQuoteRepo.GetByRentalIdAsync(pq.RentalId).Result
                .OrderBy(q => q.CreatedAt)
                .Select(q => q.Id)
                .ToList();

            var quoteNumber = allRentalQuotes.IndexOf(pq.Id) + 1;

            return new ManagerQuoteListItemResponse
            {
                Id = pq.Id,
                RentalId = pq.RentalId,
                QuoteNumber = quoteNumber,
                CustomerName = pq.Rental?.Account?.FullName ?? "Unknown",
                PackageName = pq.Rental?.ActivityType?.Name ?? "Unknown",
                EventDate = pq.Rental?.EventDate?.ToString("MMM dd, yyyy") ?? "TBD",
                // New fields
                RentalFee = pq.RentalFee,
                StaffFee = pq.StaffFee,
                DamageDeposit = pq.DamageDeposit,
                DeliveryFee = pq.DeliveryFee,
                DeliveryDistance = pq.DeliveryDistance,
                CustomizationFee = pq.CustomizationFee,
                TotalDeposit = pq.TotalDeposit,
                TotalPayment = pq.TotalPayment,
                GrandTotal = pq.GrandTotal,
                StaffDescription = pq.StaffDescription,
                ManagerFeedback = pq.ManagerFeedback,
                Status = pq.Status ?? "Unknown",
                CreatedAt = pq.CreatedAt
            };
        }).ToList();
    }

    // Helper method
    private PriceQuoteResponse MapToPriceQuoteResponse(PriceQuote quote, int quoteNumber)
    {
        return new PriceQuoteResponse
        {
            Id = quote.Id,
            RentalId = quote.RentalId,
            // LOCKED deposit components
            RentalFee = quote.RentalFee,
            StaffFee = quote.StaffFee,
            DamageDeposit = quote.DamageDeposit,
            // Adjustable fees
            DeliveryFee = quote.DeliveryFee,
            DeliveryDistance = quote.DeliveryDistance,
            CustomizationFee = quote.CustomizationFee,
            // Computed totals (from entity)
            TotalDeposit = quote.TotalDeposit,
            TotalPayment = quote.TotalPayment,
            GrandTotal = quote.GrandTotal,
            // Metadata
            StaffDescription = quote.StaffDescription,
            ManagerFeedback = quote.ManagerFeedback,
            CustomerReason = quote.CustomerReason,
            CreatedAt = quote.CreatedAt,
            Status = quote.Status,
            QuoteNumber = quoteNumber,
            ManagerId = quote.ManagerId
        };
    }

    public async Task<int> RejectActiveQuotesForRentalAsync(int rentalId)
    {
        // 1. Find all active quotes for this rental
        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(rentalId);
        var activeQuotes = allQuotes.Where(q =>
            q.IsDeleted != true &&
            (q.Status == "PendingManager" || q.Status == "PendingCustomer" || q.Status == "Approved")
        ).ToList();

        if (!activeQuotes.Any())
        {
            return 0; // No active quotes to reject
        }

        // 2. Load rental with ActivityType for recalculation
        var rental = await _rentalRepo.GetAsync(r => r.Id == rentalId, "ActivityType");
        if (rental == null)
            throw new Exception("Rental not found");

        var activityType = rental.ActivityType;
        if (activityType == null)
            throw new Exception("ActivityType not found for this rental");

        // 3. Calculate new fees based on current rental info
        var calculatedFees = CalculateQuoteFeesFromRental(rental, activityType);

        // 4. Reject all active quotes and recalculate fees (keep CustomizationFee)
        foreach (var quote in activeQuotes)
        {
            quote.Status = "RejectedManager";
            quote.ManagerFeedback = "Rental information has been updated. Please update the quote.";
            
            // Recalculate all auto-calculated fields based on current rental info
            quote.RentalFee = calculatedFees.RentalFee;
            quote.StaffFee = calculatedFees.StaffFee;
            quote.DamageDeposit = calculatedFees.DamageDeposit;
            quote.DeliveryFee = calculatedFees.DeliveryFee;
            quote.DeliveryDistance = calculatedFees.DeliveryDistance;
            
            // Keep CustomizationFee as is (staff input field)
            // CustomizationFee remains unchanged
            
            await _priceQuoteRepo.UpdateAsync(quote);
        }

        // 5. Reset flags: Rental.IsUpdated
        if (rental != null)
        {
            rental.IsUpdated = false;
            await _rentalRepo.UpdateAsync(rental);
        }

        // 6. Reset flags: RentalDetail.IsUpdated and RobotAbilityValue.isUpdated
        var rentalDetails = await _rentalDetailRepo.GetAllAsync(rd => rd.RentalId == rentalId, "RobotAbilityValues");
        
        foreach (var rentalDetail in rentalDetails)
        {
            rentalDetail.IsUpdated = false;
            
            // Reset all RobotAbilityValue.isUpdated flags
            foreach (var robotAbilityValue in rentalDetail.RobotAbilityValues)
            {
                robotAbilityValue.isUpdated = false;
                await _robotAbilityValueRepo.UpdateAsync(robotAbilityValue);
            }
            
            await _rentalDetailRepo.UpdateAsync(rentalDetail);
        }

        return activeQuotes.Count;
    }

    /// <summary>
    /// Helper method to calculate all quote fees from rental and activity type
    /// </summary>
    private (decimal RentalFee, decimal StaffFee, decimal DamageDeposit, decimal? DeliveryFee, int? DeliveryDistance) CalculateQuoteFeesFromRental(Rental rental, ActivityType activityType)
    {
        // Calculate billable hours from Rental
        decimal billableHours = 2m; // Default minimum
        if (rental.StartTime.HasValue && rental.EndTime.HasValue)
        {
            var durationMinutes = (rental.EndTime.Value.ToTimeSpan() - rental.StartTime.Value.ToTimeSpan()).TotalMinutes;
            // Apply minimum and billing increment
            durationMinutes = Math.Max(durationMinutes, activityType.MinimumMinutes);
            // Round up to billing increment
            var increment = activityType.BillingIncrementMinutes > 0 ? activityType.BillingIncrementMinutes : 30;
            durationMinutes = Math.Ceiling(durationMinutes / increment) * increment;
            billableHours = (decimal)durationMinutes / 60m;
        }

        // Calculate fees
        decimal rentalFee = activityType.HourlyRate * billableHours;
        decimal staffFee = activityType.TechnicalStaffFeePerHour * activityType.OperatorCount * billableHours;
        decimal damageDeposit = activityType.DamageDeposit;

        // Calculate DeliveryFee
        decimal? deliveryFee = null;
        int? deliveryDistance = null;

        if (!string.IsNullOrWhiteSpace(rental.City))
        {
            var (fee, distance) = CalculateDeliveryFee(rental.City);
            deliveryFee = fee;
            deliveryDistance = distance;
        }

        return (rentalFee, staffFee, damageDeposit, deliveryFee, deliveryDistance);
    }

    private (decimal Fee, int? Distance) CalculateDeliveryFee(string city)
    {
        return DeliveryFeeConfig.CalculateFee(city);
    }
}