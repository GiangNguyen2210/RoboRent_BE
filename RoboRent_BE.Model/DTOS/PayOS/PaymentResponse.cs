namespace RoboRent_BE.Model.DTOS;

public class PaymentResponse
{
    public string Code { get; set; }
    public string Desc { get; set; }
    public PaymentResponseData Data { get; set; }
}

public class PaymentResponseData
{
    public string PaymentLinkId { get; set; }
    public string CheckoutUrl { get; set; }
}