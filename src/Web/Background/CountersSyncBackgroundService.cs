using LinkyFunky.Application.Interfaces;
using LinkyFunky.Application.Interfaces.Repositories;

namespace Web.Background;

/// <summary>
/// Periodically syncs Redis counters to the persistent storage.
/// </summary>
/// <param name="scopeFactory">The scope factory used to resolve scoped services.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="configuration">The application configuration.</param>
public sealed class CountersSyncBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<CountersSyncBackgroundService> logger,
    IConfiguration configuration
) : BackgroundService
{
    const int DefaultSyncIntervalMinutes = 5;
    const string SyncIntervalMinutesKey = "Counters:SyncIntervalMinutes";

    /// <summary>
    /// Executes periodic counter synchronization.
    /// </summary>
    /// <param name="ctk">The token used to stop the background process.</param>
    protected override async Task ExecuteAsync(CancellationToken ctk)
    {
        var intervalMinutes = configuration.GetValue<int?>(SyncIntervalMinutesKey) ?? DefaultSyncIntervalMinutes;
        var interval = TimeSpan.FromMinutes(intervalMinutes);
        using var timer = new PeriodicTimer(interval);

        while (!ctk.IsCancellationRequested)
        {
            try
            {
                var synced = await SyncOnceAsync(ctk);
                
                if (synced > 0)
                    logger.LogInformation("{counters} counters synchronization completed successfully.", synced);
            }
            catch (OperationCanceledException) when (ctk.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Counter synchronization iteration failed.");
            }

            if (!await timer.WaitForNextTickAsync(ctk))
                break;
        }
    }

    /// <summary>
    /// Synchronizes counters once.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The number of counters that were successfully synchronized.</returns>
    async Task<int> SyncOnceAsync(CancellationToken ctk)
    {
        using var scope = scopeFactory.CreateScope();
        var counterService = scope.ServiceProvider.GetRequiredService<ICounterService>();
        var counters = await counterService.GetAllAsync(ctk);
        
        if (counters.Count == 0)
            return 0;

        var persistedKeys = await PushCountersToDatabaseAsync(counters, ctk);
        if (persistedKeys.Count == 0)
            return 0;

        await counterService.RemoveAllAsync(persistedKeys, ctk);
        return counters.Count;
    }

    /// <summary>
    /// Pushes counters to the database.
    /// </summary>
    /// <param name="counters">Counters collected from Redis.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>Keys that were successfully persisted and can be removed from Redis.</returns>
    async Task<IReadOnlyCollection<string>> PushCountersToDatabaseAsync(
        IReadOnlyCollection<Counter> counters,
        CancellationToken ctk)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        
        var shortcutsRepository = scope.ServiceProvider.GetRequiredService<IShortcutsRepository>();
        var counterByCode = counters.ToDictionary(
            x => x.Key, 
            x => x.Value, 
            StringComparer.Ordinal);
        
        await shortcutsRepository.UpdateRedirectsAsync(counterByCode, ctk);
        await shortcutsRepository.UnitOfWork.SaveChangesAsync(ctk);
        
        return counters.Select(x => x.Key).ToList();
    }
}
