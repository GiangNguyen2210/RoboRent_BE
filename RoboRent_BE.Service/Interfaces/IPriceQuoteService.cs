using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface IPriceQuoteService
{
    Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId);
    Task<PriceQuoteResponse> GetPriceQuoteAsync(int id);
    Task<RentalQuotesResponse> GetQuotesByRentalIdAsync(int rentalId);
    Task<PriceQuoteResponse> CustomerActionAsync(int quoteId, CustomerActionRequest request, int customerId);
    Task<PriceQuoteResponse> ManagerActionAsync(int quoteId, ManagerActionRequest request, int managerId);
    Task<PriceQuoteResponse> UpdatePriceQuoteAsync(int quoteId, UpdatePriceQuoteRequest request, int staffId);
    Task<List<ManagerQuoteListItemResponse>> GetAllQuotesForManagerAsync(string? status = null);

}