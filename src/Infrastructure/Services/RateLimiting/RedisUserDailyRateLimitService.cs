using LinkyFunky.Application.Interfaces;
using LinkyFunky.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LinkyFunky.Infrastructure.Services.RateLimiting;

/// <summary>
/// Redis-backed per-user daily rate limiting using a fixed UTC calendar day window.
/// </summary>
/// <param name="connectionMultiplexer">Redis connection multiplexer.</param>
/// <param name="rateOptions">Configured limits per operation kind.</param>
/// <param name="configuration">Application configuration for Redis key prefixing.</param>
public sealed class RedisUserDailyRateLimitService(
    IConnectionMultiplexer connectionMultiplexer,
    IOptions<RateLimitOptions> rateOptions,
    IConfiguration configuration) : IUserDailyRateLimitService
{
    const string RedisInstanceNameKey = "Redis:InstanceName";
    const string RateLimitKeyNamespace = "ratelimit:";

    /// <summary>
    /// Atomically increments the daily counter and either accepts (within limit) or rolls back when over quota.
    /// </summary>
    const string LuaAcquire = """
        local limit = tonumber(ARGV[1])
        local ttlSeconds = tonumber(ARGV[2])
        local current = redis.call('INCR', KEYS[1])
        if current == 1 then
          redis.call('EXPIRE', KEYS[1], ttlSeconds)
        end
        if current > limit then
          redis.call('DECR', KEYS[1])
          return -1
        end
        return limit - current
        """;

    readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    readonly string _keyPrefix = BuildKeyPrefix(configuration);

    /// <inheritdoc />
    public async Task<RateLimitAcquireResult> TryAcquireAsync(Guid userId, UserRateLimitKind kind, CancellationToken ctk)
    {
        var options = rateOptions.Value;
        var limit = kind switch
        {
            UserRateLimitKind.CreateShortcut => options.CreateShortcutPerUtcDay,
            UserRateLimitKind.RedirectShortcut => options.RedirectShortcutPerUtcDay,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        if (limit <= 0)
            return new(true, limit, int.MaxValue, null);

        var utcNow = DateTime.UtcNow;
        var ttlSeconds = UtcCalendarDayRateLimit.GetSecondsUntilNextUtcDay(utcNow);
        var key = BuildRedisKey(kind, userId, utcNow);

        var redisResult = await _database.ScriptEvaluateAsync(
                LuaAcquire,
                [key],
                [limit, ttlSeconds])
            .WaitAsync(ctk);

        var numeric = (long)redisResult;

        if (numeric < 0)
        {
            var retryAfter = TimeSpan.FromSeconds(UtcCalendarDayRateLimit.GetSecondsUntilNextUtcDay(utcNow));
            return new(false, limit, 0, retryAfter);
        }

        var remaining = (int)numeric;
        return new(true, limit, remaining, null);
    }

    static string BuildKeyPrefix(IConfiguration configuration)
    {
        var instance = configuration[RedisInstanceNameKey];
        if (string.IsNullOrWhiteSpace(instance))
            return RateLimitKeyNamespace;

        return $"{instance.TrimEnd(':')}:{RateLimitKeyNamespace}";
    }

    /// <summary>
    /// Builds a Redis key scoped by operation, user, and UTC date so each calendar day uses a separate counter.
    /// </summary>
    RedisKey BuildRedisKey(UserRateLimitKind kind, Guid userId, DateTime utcNow)
    {
        var kindSegment = kind switch
        {
            UserRateLimitKind.CreateShortcut => "create",
            UserRateLimitKind.RedirectShortcut => "redirect",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };

        var day = utcNow.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        return $"{_keyPrefix}{kindSegment}:{userId:N}:{day}";
    }
}
