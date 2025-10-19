using RoboRent_BE.Model.DTOs.PriceQuote;

namespace RoboRent_BE.Service.Interfaces;

public interface IPriceQuoteService
{
    Task<PriceQuoteResponse> CreatePriceQuoteAsync(CreatePriceQuoteRequest request, int staffId);
    Task<PriceQuoteResponse> GetPriceQuoteAsync(int id);
    Task<RentalQuotesResponse> GetQuotesByRentalIdAsync(int rentalId);
}