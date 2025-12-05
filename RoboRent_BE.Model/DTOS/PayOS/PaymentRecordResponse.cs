namespace RoboRent_BE.Model.DTOS;

/// <summary>
/// Response DTO cho PaymentRecord
/// </summary>
public class PaymentRecordResponse
{
    public int Id { get; set; }
    
    public int? RentalId { get; set; }
    
    /// <summary>
    /// Tên sự kiện của Rental (để hiển thị trên UI)
    /// </summary>
    public string? RentalName { get; set; }
    
    public int? PriceQuoteId { get; set; }
    
    /// <summary>
    /// Loại thanh toán: "Deposit", "Full", "ContractReportResolution"
    /// </summary>
    public string? PaymentType { get; set; }
    
    /// <summary>
    /// Số tiền thanh toán (VND)
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Mã order duy nhất từ PayOS
    /// </summary>
    public long OrderCode { get; set; }
    
    /// <summary>
    /// ID của payment link trong hệ thống PayOS
    /// </summary>
    public string? PaymentLinkId { get; set; }
    
    /// <summary>
    /// Trạng thái thanh toán: "Pending", "Paid", "Failed", "Cancelled", "Processing"
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Thời gian tạo payment record
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Thời gian thanh toán thành công (nullable)
    /// </summary>
    public DateTime? PaidAt { get; set; }
    
    /// <summary>
    /// ✅ URL để customer thanh toán (PayOS checkout page)
    /// Nullable vì payment records cũ có thể không có field này
    /// </summary>
    public string? CheckoutUrl { get; set; }
    
    /// <summary>
    /// ✅ Hạn sử dụng của payment link
    /// Nullable vì payment records cũ có thể không có field này
    /// </summary>
    public DateTime? ExpiredAt { get; set; }
    
    /// <summary>
    /// Helper property: Kiểm tra payment đã hết hạn chưa
    /// </summary>
    public bool IsExpired => ExpiredAt.HasValue && ExpiredAt.Value < DateTime.UtcNow;
    
    /// <summary>
    /// Helper property: Kiểm tra payment có thể thanh toán không
    /// </summary>
    public bool CanPay => Status == "Pending" && !IsExpired && !string.IsNullOrEmpty(CheckoutUrl);
}

/// <summary>
/// DTO cho thông tin deposit payment (dùng trong ContractDraftsResponse)
/// </summary>
public class DepositPaymentInfo
{
    public long OrderCode { get; set; }
    public decimal Amount { get; set; }
    public string CheckoutUrl { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// DTO cho thông tin full payment (dùng trong RentalCompletionResponse)
/// </summary>
public class FullPaymentInfo
{
    public long OrderCode { get; set; }
    public decimal Amount { get; set; }
    public string CheckoutUrl { get; set; }
    public DateTime ExpiresAt { get; set; }
}