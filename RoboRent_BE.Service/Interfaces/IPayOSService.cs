using Net.payOS.Types;
using RoboRent_BE.Model.DTOS;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoboRent_BE.Model.DTOS.Account;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interface;

public interface IPayOSService
{
    Task<PaymentResponse> CreatePaymentLink(PaymentRequest request, int accountId);
    Task<PaymentLinkInformation> GetPaymentLinkInformation(long orderCode);
    Task<PaymentLinkInformation> CancelPaymentLink(long orderCode, string cancellationReason = null);
    WebhookData VerifyWebhookData(WebhookType webhookBody);
    Task<List<PaymentHistory>> GetPaymentTransactionByAccountId(int accountId);
    Task<AccountDto?> GetAccountByIdAsync(int accountId);
    Task<PaymentTransactionDto?> GetTransactionByOrderCodeAsync(long orderCode);
    Task UpdateTransactionStatusAsync(long orderCode, string status);
    Task UpdateAccountSubscriptionAsync(int accountId, int amount);
}