namespace LinkyFunky.Infrastructure.Options;

/// <summary>
/// Per-user daily rate limit quotas aligned with the product requirements (UTC calendar day).
/// </summary>
public sealed class RateLimitOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "RateLimits";

    /// <summary>
    /// Gets or sets the maximum number of shortcut creations per user per UTC day.
    /// </summary>
    public int CreateShortcutPerUtcDay { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum number of shortcut redirects per user per UTC day.
    /// </summary>
    public int RedirectShortcutPerUtcDay { get; set; } = 50;
}
