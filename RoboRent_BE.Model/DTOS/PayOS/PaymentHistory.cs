namespace RoboRent_BE.Model.DTOS;

public class PaymentHistory
{
    public string Description { get; set; } // Mô tả giao dịch

    public int Amount { get; set; } // Số tiền

    public string Status { get; set; } // Trạng thái: PENDING, SUCCESS, FAILED
    
    public DateTime CreatedAt { get; set; } // Thời gian tạo giao dịch
}