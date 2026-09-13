using ServiceC.DataCleaning;

namespace ServiceC.Services;

public class DataCleaner(
    IServiceScopeFactory scopeFactory,
    ILogger<DataCleaner> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await CleanAsync(stoppingToken);
        
        using var timer = new PeriodicTimer(
            TimeSpan.FromHours(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanAsync(stoppingToken);
        }
    }

    private async Task CleanAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();

            var cleanupService = scope.ServiceProvider
                .GetRequiredService<IDataCleanupService>();

            await cleanupService.DeleteOldRecordsAsync(
                cancellationToken);

            logger.LogInformation(
                "Old weather records cleanup completed");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error while cleaning old weather records");
        }
    }
}