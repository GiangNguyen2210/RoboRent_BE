using RoboRent_BE.Model.Entities;
using System.Linq.Expressions;
using RoboRent_BE.Model.DTOS;
using RoboRent_BE.Repository.Interface;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Interface;

public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
{
    Task<IEnumerable<PaymentTransaction>> GetByAccountIdAsync(int accountId);
    Task<long> GetLastOrderCodeAsync();
    Task<PaymentTransaction?> GetByOrderCodeAsync(long orderCode);
}