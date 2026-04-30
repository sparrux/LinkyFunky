using LinkyFunky.Infrastructure.Services.RateLimiting;

namespace Infrastructure.UnitTests.Services.RateLimiting;

/// <summary>
/// Tests UTC calendar-day helpers used for Redis TTL and Retry-After headers.
/// </summary>
public sealed class UtcCalendarDayRateLimitTests
{
    [Fact]
    public void GetSecondsUntilNextUtcDay_one_second_before_midnight_returns_one()
    {
        var utcNow = new DateTime(2026, 4, 30, 23, 59, 59, DateTimeKind.Utc);

        var seconds = UtcCalendarDayRateLimit.GetSecondsUntilNextUtcDay(utcNow);

        Assert.Equal(1, seconds);
    }

    [Fact]
    public void GetTimeSpanUntilNextUtcDay_at_midnight_returns_full_day()
    {
        var utcNow = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        var ts = UtcCalendarDayRateLimit.GetTimeSpanUntilNextUtcDay(utcNow);

        Assert.Equal(TimeSpan.FromDays(1), ts);
    }
}
