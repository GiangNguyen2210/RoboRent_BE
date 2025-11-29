namespace RoboRent_BE.Model.DTOS;

public class PaymentRecordResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public int? PriceQuoteId { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public long OrderCode { get; set; }
    public string? PaymentLinkId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? CheckoutUrl { get; set; } // Chỉ có khi mới tạo
}