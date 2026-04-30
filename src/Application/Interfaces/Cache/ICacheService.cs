namespace LinkyFunky.Application.Interfaces.Cache;

/// <summary>
/// Defines a contract for caching operations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Caches a value using default expiration settings.
    /// </summary>
    /// <typeparam name="T">The value type to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    Task SetAsync<T>(string key, T value, CancellationToken ctk);

    /// <summary>
    /// Caches a value using an optional custom expiration.
    /// </summary>
    /// <typeparam name="T">The value type to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiry">The optional absolute expiration relative to now.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry, CancellationToken ctk);
    
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The value type to read.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The cached value or <c>null</c> if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken ctk);
}