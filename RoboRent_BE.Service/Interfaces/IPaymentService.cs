using Net.payOS.Types;
using RoboRent_BE.Model.DTOS;

namespace RoboRent_BE.Service.Interface;

public interface IPaymentService
{
    // Payment Creation
    Task<PaymentRecordResponse> CreateDepositPaymentAsync(int rentalId);
    Task<PaymentRecordResponse> CreateFullPaymentAsync(int rentalId);
    
    // Payment Retrieval
    Task<List<PaymentRecordResponse>> GetPaymentsByRentalIdAsync(int rentalId);
    Task<List<PaymentRecordResponse>> GetCustomerTransactionsAsync(int customerId);
    
    // Webhook Processing
    Task ProcessWebhookAsync(long orderCode, string paymentStatus);
    WebhookData VerifyPaymentWebhook(WebhookType webhookBody);
    
    // PayOS Integration (moved from PayOSService)
    Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode);
    Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderCode, string cancellationReason = null);
    
    // Expiration
    Task ExpirePendingPaymentsAsync();
}