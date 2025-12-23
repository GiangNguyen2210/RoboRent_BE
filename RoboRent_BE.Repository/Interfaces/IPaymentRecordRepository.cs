using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface IPaymentRecordRepository : IGenericRepository<PaymentRecord>
{
    Task<PaymentRecord?> GetByOrderCodeAsync(long orderCode);
    Task<List<PaymentRecord>> GetByRentalIdAsync(int rentalId);
    Task<PaymentRecord?> GetByRentalIdAndTypeAsync(int rentalId, string paymentType);
    Task<long> GetLastOrderCodeAsync();
    Task<IEnumerable<PaymentRecord>> GetExpiredPaymentRecordsAsync();
}