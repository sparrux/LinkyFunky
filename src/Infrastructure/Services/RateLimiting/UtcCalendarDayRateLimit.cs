namespace LinkyFunky.Infrastructure.Services.RateLimiting;

/// <summary>
/// Helpers for fixed UTC calendar-day rate limit windows and HTTP Retry-After values.
/// </summary>
public static class UtcCalendarDayRateLimit
{
    /// <summary>
    /// Returns the time span from <paramref name="utcNow"/> until 00:00:00 of the next UTC day.
    /// </summary>
    public static TimeSpan GetTimeSpanUntilNextUtcDay(DateTime utcNow)
    {
        var startOfNext = utcNow.Date.AddDays(1);
        return startOfNext - utcNow;
    }

    /// <summary>
    /// Returns a positive number of whole seconds until the next UTC day, suitable for EXPIRE and Retry-After.
    /// </summary>
    public static int GetSecondsUntilNextUtcDay(DateTime utcNow)
    {
        var seconds = (int)Math.Ceiling(GetTimeSpanUntilNextUtcDay(utcNow).TotalSeconds);
        return Math.Max(seconds, 1);
    }
}
