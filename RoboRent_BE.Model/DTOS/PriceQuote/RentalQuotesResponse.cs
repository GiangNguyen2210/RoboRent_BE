namespace RoboRent_BE.Model.DTOs.PriceQuote;

// Response danh sách quotes của 1 rental
public class RentalQuotesResponse
{
    public int RentalId { get; set; }
    public List<PriceQuoteResponse> Quotes { get; set; } = new();
    public int TotalQuotes { get; set; }
    public bool CanCreateMore { get; set; } // false nếu đã có 3 quotes
}