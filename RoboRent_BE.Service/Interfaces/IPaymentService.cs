using RoboRent_BE.Model.DTOS;

namespace RoboRent_BE.Service.Interface;

public interface IPaymentService
{
    Task<PaymentRecordResponse> CreateDepositPaymentAsync(int rentalId);
    Task<PaymentRecordResponse> CreateFullPaymentAsync(int rentalId);
    Task<List<PaymentRecordResponse>> GetPaymentsByRentalIdAsync(int rentalId);
    Task ProcessWebhookAsync(long orderCode, string paymentStatus);
    Task<List<PaymentRecordResponse>> GetCustomerTransactionsAsync(int customerId);
}