using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RoboRent_BE.Service.Interface;

namespace RoboRent_BE.Service.Services;

public class PaymentRecordsExpirationService : BackgroundService
{
    private readonly ILogger<PaymentRecordsExpirationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1); // Check every day

    public PaymentRecordsExpirationService(
        ILogger<PaymentRecordsExpirationService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Records Expiration Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                    await paymentService.ExpirePendingPaymentsAsync();
                }

                _logger.LogInformation("Checked for expired payment records. Next check in 1 day.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for expired payment records.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Payment Records Expiration Service is stopping.");
    }
}

