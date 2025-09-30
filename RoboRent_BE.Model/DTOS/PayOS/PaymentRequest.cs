using Net.payOS.Types;

namespace RoboRent_BE.Model.DTOS;

public class PaymentRequest
{
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; }
    public string BuyerName { get; set; }
    public string BuyerEmail { get; set; }
    public string BuyerPhone { get; set; }
    public List<ItemData>? Items { get; set; } // Thêm danh sách sản phẩm
    public int? ExpiredAt { get; set; } // Thời gian hết hạn link thanh toán
}