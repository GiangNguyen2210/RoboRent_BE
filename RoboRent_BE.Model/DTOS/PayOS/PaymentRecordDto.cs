namespace RoboRent_BE.Model.DTOS;

public class PaymentRecordDto
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public long OrderCode { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}