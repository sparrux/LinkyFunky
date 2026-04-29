using LinkyFunky.Application.Interfaces;
using StackExchange.Redis;

namespace LinkyFunky.Infrastructure.Services.Counters;

/// <summary>
/// Provides Redis-backed counter operations that are safe for multi-instance usage.
/// </summary>
/// <param name="connectionMultiplexer">The Redis multiplexer instance.</param>
public sealed class RedisCounterService(IConnectionMultiplexer connectionMultiplexer) : ICounterService
{
    const string CounterPrefix = "counter:";
    readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    /// <summary>
    /// Increments a counter atomically using Redis INCR command.
    /// </summary>
    /// <param name="key">The counter key.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    public async Task IncrementAsync(string key, CancellationToken ctk = default)
    {
        var normalizedKey = BuildKey(key);
        await _database.StringIncrementAsync(normalizedKey, flags: CommandFlags.FireAndForget).WaitAsync(ctk);
    }

    /// <summary>
    /// Returns the current counter value by key.
    /// </summary>
    /// <param name="key">The counter key.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The current counter value or zero when key is missing.</returns>
    public async Task<int> GetAsync(string key, CancellationToken ctk = default)
    {
        var normalizedKey = BuildKey(key);
        var value = await _database.StringGetAsync(normalizedKey).WaitAsync(ctk);
        if (value.IsNullOrEmpty)
            return 0;

        if (value.TryParse(out int counter))
            return counter;

        throw new InvalidOperationException($"Counter value for key '{normalizedKey}' is not a valid integer.");
    }

    public Task<IReadOnlyCollection<Counter>> GetAllAsync(CancellationToken ctk = default)
    {
        var endpoints = connectionMultiplexer.GetEndPoints(configuredOnly: true);
        if (endpoints.Length == 0)
            return Task.FromResult<IReadOnlyCollection<Counter>>([]);

        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var endpoint in endpoints)
        {
            var server = connectionMultiplexer.GetServer(endpoint);
            if (!server.IsConnected)
                continue;

            foreach (var key in server.Keys(pattern: $"{CounterPrefix}*"))
                keys.Add(key.ToString());
        }

        if (keys.Count == 0)
            return Task.FromResult<IReadOnlyCollection<Counter>>([]);

        return GetAllValuesAsync(keys.ToArray(), ctk);
    }

    /// <summary>
    /// Removes multiple counters from Redis storage.
    /// </summary>
    /// <param name="keys">The counter keys without prefix.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    public async Task RemoveAllAsync(IEnumerable<string> keys, CancellationToken ctk = default)
    {
        var redisKeys = keys
            .Select(BuildKey)
            .Distinct(StringComparer.Ordinal)
            .Select(static key => (RedisKey)key)
            .ToArray();

        if (redisKeys.Length == 0)
            return;

        await _database.KeyDeleteAsync(redisKeys).WaitAsync(ctk);
    }

    async Task<IReadOnlyCollection<Counter>> GetAllValuesAsync(string[] keys, CancellationToken ctk)
    {
        var redisKeys = keys.Select(static k => (RedisKey)k).ToArray();
        var values = await _database.StringGetAsync(redisKeys).WaitAsync(ctk);
        var counters = new List<Counter>(values.Length);

        for (var index = 0; index < values.Length; index++)
        {
            ctk.ThrowIfCancellationRequested();

            var value = values[index];
            if (value.IsNullOrEmpty || !value.TryParse(out int counter))
                continue;

            var rawKey = keys[index];
            counters.Add(new Counter(rawKey[CounterPrefix.Length..], counter));
        }

        return counters;
    }

    static string ValidateKey(string key)
    {
        var normalizedKey = key.Trim();
        if (string.IsNullOrWhiteSpace(normalizedKey))
            throw new ArgumentException("Counter key cannot be empty.", nameof(key));

        return normalizedKey;
    }

    static string BuildKey(string key)
    {
        var normalized = ValidateKey(key);
        return $"{CounterPrefix}{normalized}";
    }
}