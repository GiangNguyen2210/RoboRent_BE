using RoboRent_BE.Model.DTOs.PriceQuote;

namespace RoboRent_BE.Service.Interfaces;

public interface IPriceQuoteService
{
    Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId);
    Task<PriceQuoteResponse> GetPriceQuoteAsync(int id);
    Task<RentalQuotesResponse> GetQuotesByRentalIdAsync(int rentalId);
    Task<PriceQuoteResponse> AcceptQuoteAsync(int quoteId, int customerId); 
    Task<PriceQuoteResponse> ApproveQuoteAsync(int quoteId, int managerId);
    Task<PriceQuoteResponse> RejectQuoteAsync(int quoteId, string feedback, int managerId);
    Task<PriceQuoteResponse> ApproveQuoteByCustomerAsync(int quoteId, int customerId);
    Task<PriceQuoteResponse> RejectQuoteByCustomerAsync(int quoteId, string? reason, int customerId);

}