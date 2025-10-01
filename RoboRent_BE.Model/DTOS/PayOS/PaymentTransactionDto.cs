namespace RoboRent_BE.Model.DTOS;

public class PaymentTransactionDto
{
    public int Id { get; set; }
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AccountId { get; set; } // Để lấy AccountId cho subscription
    // Thêm fields khác nếu cần (e.g., Description)

}