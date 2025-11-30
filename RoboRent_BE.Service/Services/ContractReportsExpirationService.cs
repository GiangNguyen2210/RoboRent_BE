using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ContractReportsExpirationService : BackgroundService
{
    private readonly ILogger<ContractReportsExpirationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public ContractReportsExpirationService(
        ILogger<ContractReportsExpirationService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Contract Reports Expiration Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var contractReportsService = scope.ServiceProvider.GetRequiredService<IContractReportsService>();
                    await contractReportsService.ExpirePendingReportsAsync();
                }

                _logger.LogInformation("Checked for expired contract reports. Next check in 1 hour.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for expired contract reports.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Contract Reports Expiration Service is stopping.");
    }
}

