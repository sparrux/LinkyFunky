using LinkyFunky.Infrastructure.Services.RateLimiting;

namespace Infrastructure.UnitTests.Services.RateLimiting;

/// <summary>
/// Tests UTC calendar-day helpers used for Redis TTL and Retry-After headers.
/// </summary>
public sealed class UtcCalendarDayRateLimitTests
{
    [Fact]
    public void GetSecondsUntilNextUtcDay_OneSecondBeforeMidnight_ReturnsOne()
    {
        var utcNow = new DateTime(2026, 4, 30, 23, 59, 59, DateTimeKind.Utc);

        var seconds = UtcCalendarDayRateLimit.GetSecondsUntilNextUtcDay(utcNow);

        Assert.Equal(1, seconds);
    }

    [Fact]
    public void GetTimeSpanUntilNextUtcDay_AtMidnight_ReturnsFullDay()
    {
        var utcNow = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        var ts = UtcCalendarDayRateLimit.GetTimeSpanUntilNextUtcDay(utcNow);

        Assert.Equal(TimeSpan.FromDays(1), ts);
    }
}
