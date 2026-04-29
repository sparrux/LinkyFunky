namespace LinkyFunky.Application.Interfaces.Cache;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, CancellationToken ctk);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry, CancellationToken ctk);
    Task<T?> GetAsync<T>(string key, CancellationToken ctk);
}