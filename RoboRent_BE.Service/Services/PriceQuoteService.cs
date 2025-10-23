using RoboRent_BE.Model.DTOs.PriceQuote;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class PriceQuoteService : IPriceQuoteService
{
    private readonly IPriceQuoteRepository _priceQuoteRepo;

    public PriceQuoteService(IPriceQuoteRepository priceQuoteRepo)
    {
        _priceQuoteRepo = priceQuoteRepo;
    }

    public async Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId)
    {
        // ✅ Business Rule: Check max 3 quotes per rental
        var existingQuotesCount = await _priceQuoteRepo.CountByRentalIdAsync(request.RentalId);
        
        if (existingQuotesCount >= 3)
        {
            throw new Exception($"Maximum 3 quotes reached for rental {request.RentalId}. Cannot create more.");
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
            Status = "Pending",
            IsDeleted = false
        };

        await _priceQuoteRepo.AddAsync(quote);

        // ✅ Không tự gửi chat notification nữa - Controller sẽ lo
        return MapToPriceQuoteResponse(quote, existingQuotesCount + 1);
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
    
    public async Task<PriceQuoteResponse> AcceptQuoteAsync(int quoteId, int customerId)
    {
        var quote = await _priceQuoteRepo.GetAsync(pq => 
            pq.Id == quoteId && 
            pq.IsDeleted != true);
    
        if (quote == null)
        {
            throw new Exception("Price quote not found");
        }
    
        // Business rule: Can only accept if status is Pending
        if (quote.Status != "Pending")
        {
            throw new Exception($"Cannot accept quote with status: {quote.Status}");
        }
    
        // ✅ 1. Accept this quote
        quote.Status = "Accepted";
        await _priceQuoteRepo.UpdateAsync(quote);
    
        // ✅ 2. Auto reject other pending quotes of same rental
        var otherPendingQuotes = await _priceQuoteRepo
            .GetAllAsync(q => 
                q.RentalId == quote.RentalId && 
                q.Id != quoteId && 
                q.Status == "Pending" &&
                q.IsDeleted != true);
    
        foreach (var otherQuote in otherPendingQuotes)
        {
            otherQuote.Status = "Rejected";
            await _priceQuoteRepo.UpdateAsync(otherQuote);
        }
    
        // Get quote number for response
        var allQuotes = await _priceQuoteRepo.GetByRentalIdAsync(quote.RentalId);
        var quoteNumber = allQuotes
            .OrderBy(q => q.CreatedAt)
            .ToList()
            .FindIndex(q => q.Id == quoteId) + 1;
    
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
            QuoteNumber = quoteNumber
        };
    }
}