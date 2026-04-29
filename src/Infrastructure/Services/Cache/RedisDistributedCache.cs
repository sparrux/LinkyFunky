using LinkyFunky.Application.Interfaces.Cache;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LinkyFunky.Infrastructure.Services.Cache;

/// <summary>
/// Provides Redis-backed cache operations for application use cases.
/// </summary>
/// <param name="distributedCache">The distributed cache instance.</param>
public sealed class RedisDistributedCache(IDistributedCache distributedCache) : ICacheService
{
    static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    static readonly DistributedCacheEntryOptions DefaultEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromHours(12)
    };

    /// <summary>
    /// Caches a value using default expiration settings.
    /// </summary>
    /// <typeparam name="T">The value type to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    public Task SetAsync<T>(string key, T value, CancellationToken ctk)
    {
        return SetInternalAsync(key, value, DefaultEntryOptions, ctk);
    }

    /// <summary>
    /// Caches a value using an optional custom expiration.
    /// </summary>
    /// <typeparam name="T">The value type to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiry">The optional absolute expiration relative to now.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry, CancellationToken ctk)
    {
        var entryOptions = expiry is null
            ? DefaultEntryOptions
            : new()
            {
                AbsoluteExpirationRelativeToNow = expiry
            };

        return SetInternalAsync(key, value, entryOptions, ctk);
    }

    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The value type to read.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The cached value or <c>null</c> if not found.</returns>
    public async Task<T?> GetAsync<T>(string key, CancellationToken ctk)
    {
        var payload = await distributedCache.GetStringAsync(key, ctk);
        if (string.IsNullOrWhiteSpace(payload))
            return default;
        
        return JsonSerializer.Deserialize<T>(payload, SerializerOptions);
    }

    Task SetInternalAsync<T>(string key, T value, DistributedCacheEntryOptions entryOptions, CancellationToken ctk)
    {
        var payload = JsonSerializer.Serialize(value, SerializerOptions);
        return distributedCache.SetStringAsync(key, payload, entryOptions, ctk);
    }
}