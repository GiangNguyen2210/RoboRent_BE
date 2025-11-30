namespace RoboRent_BE.Model.DTOS.RentalOrder;

public class RentalCompletionResponse
{
    public OrderResponse Rental { get; set; }
    public FullPaymentInfo FullPayment { get; set; }
}

public class FullPaymentInfo
{
    public long OrderCode { get; set; }
    public decimal Amount { get; set; }
    public string CheckoutUrl { get; set; }
    public DateTime ExpiresAt { get; set; }
}