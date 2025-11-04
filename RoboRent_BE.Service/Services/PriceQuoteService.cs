using RoboRent_BE.Model.DTOs.PriceQuote;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class PriceQuoteService : IPriceQuoteService
{
    private readonly IPriceQuoteRepository _priceQuoteRepo;
    private readonly IRentalRepository _rentalRepo; 

    public PriceQuoteService(
        IPriceQuoteRepository priceQuoteRepo,
        IRentalRepository rentalRepo)
    {
        _priceQuoteRepo = priceQuoteRepo;
        _rentalRepo = rentalRepo;
    }

    public async Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId)
    {
        // Validate rental status FIRST
        var rental = await _rentalRepo.GetAsync(r => r.Id == request.RentalId);
        if (rental.Status != "AcceptedDemo")
            throw new Exception("Chỉ tạo quote sau khi demo được chấp nhận");
        
        // ✅ Business Rule: Max 3 quotes per rental
        var existingQuotes = await _priceQuoteRepo.GetByRentalIdAsync(request.RentalId);

        if (existingQuotes.Count >= 3)
        {
            throw new Exception($"Maximum 3 quotes reached for rental {request.RentalId}. Cannot create more.");
        }

        // Check không có quote nào đang active
        var activeQuote = existingQuotes.FirstOrDefault(q => 
            q.Status == "PendingManager" || 
            q.Status == "PendingCustomer" || 
            q.Status == "Approved");

        if (activeQuote != null)
        {
            throw new Exception($"Cannot create new quote. Active quote exists with status: {activeQuote.Status}");
        }

        // Create new quote
        var quote = new PriceQuote
        {
            RentalId = request.RentalId,
            Delivery = request.Delivery,
            Deposit = request.Deposit,
            Complete = request.Complete,
            Service = request.Service,
            StaffDescription = request.StaffDescription,
            ManagerFeedback = request.ManagerFeedback,
            CreatedAt = DateTime.UtcNow,
            Status = "PendingManager",
            SubmittedToManagerAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _priceQuoteRepo.AddAsync(quote);
        
        rental.Status = "PendingPriceQuote";
        await _rentalRepo.UpdateAsync(rental);

        // ✅ Không tự gửi chat notification nữa - Controller sẽ lo
        return MapToPriceQuoteResponse(quote, existingQuotes.Count + 1);
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

        return new RentalQuotesResponse
        {
            RentalId = rentalId,
            Quotes = quoteResponses,
            TotalQuotes = quotes.Count,
            CanCreateMore = quotes.Count < 3
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
            // Accept this quote
            quote.Status = "Approved";
            await _priceQuoteRepo.UpdateAsync(quote);
        
            // Update rental status
            var rental = await _rentalRepo.GetAsync(r => r.Id == quote.RentalId);
            rental.Status = "AcceptedPriceQuote";
            await _rentalRepo.UpdateAsync(rental);

            // Auto reject other pending quotes
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
            quote.Status = "RejectedCustomer";
            await _priceQuoteRepo.UpdateAsync(quote);
        }
        else
        {
            throw new Exception("Invalid action. Use 'approve' or 'reject'");
        }

        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        var quoteNumber = allQuotes.OrderBy(q => q.CreatedAt).ToList().FindIndex(q => q.Id == quoteId) + 1;
    
        return MapToPriceQuoteResponse(quote, quoteNumber);
    }

    // Helper method
    private PriceQuoteResponse MapToPriceQuoteResponse(PriceQuote quote, int quoteNumber)
    {
        var total = (quote.Delivery ?? 0) + (quote.Deposit ?? 0) + (quote.Complete ?? 0) + (quote.Service ?? 0);

        return new PriceQuoteResponse
        {
            Id = quote.Id,
            RentalId = quote.RentalId,
            Delivery = quote.Delivery,
            Deposit = quote.Deposit,
            Complete = quote.Complete,
            Service = quote.Service,
            Total = total,
            StaffDescription = quote.StaffDescription,
            ManagerFeedback = quote.ManagerFeedback,
            CreatedAt = quote.CreatedAt,
            Status = quote.Status,
            QuoteNumber = quoteNumber,
            ManagerId = quote.ManagerId
        };
    }
}