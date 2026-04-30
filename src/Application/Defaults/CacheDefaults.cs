namespace LinkyFunky.Application.Defaults;

/// <summary>
/// Provides default cache settings for application use cases.
/// </summary>
public static class CacheDefaults
{
    /// <summary>
    /// Gets the default expiration time for shortcut cache entries.
    /// </summary>
    public static TimeSpan ShortcutsExpiry => TimeSpan.FromHours(12);
    
    /// <summary>
    /// Gets the cache key for a long URL by its short code.
    /// </summary>
    /// <param name="shortCode">The short code of the shortcut.</param>
    /// <returns>The cache key for the long URL.</returns>
    public static string LongUrlKey(string shortCode) => "long-url:" + shortCode;
}