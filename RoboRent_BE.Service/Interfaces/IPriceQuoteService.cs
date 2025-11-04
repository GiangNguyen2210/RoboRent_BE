using RoboRent_BE.Model.DTOs.PriceQuote;

namespace RoboRent_BE.Service.Interfaces;

public interface IPriceQuoteService
{
    Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId);
    Task<PriceQuoteResponse> GetPriceQuoteAsync(int id);
    Task<RentalQuotesResponse> GetQuotesByRentalIdAsync(int rentalId);
    Task<PriceQuoteResponse> AcceptQuoteAsync(int quoteId, int customerId); 
    Task<PriceQuoteResponse> ApproveQuoteByCustomerAsync(int quoteId, int customerId);
    Task<PriceQuoteResponse> RejectQuoteByCustomerAsync(int quoteId, string? reason, int customerId);
    Task<PriceQuoteResponse> ManagerActionAsync(int quoteId, ManagerActionRequest request, int managerId);

}