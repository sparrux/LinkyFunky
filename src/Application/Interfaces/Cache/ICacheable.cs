namespace LinkyFunky.Application.Interfaces.Cache;

/// <summary>
/// Defines a contract for objects that can be cached.
/// </summary>
public interface ICacheable
{
    /// <summary>
    /// Gets the expiration time for the cached object.
    /// </summary>
    TimeSpan? Expiry { get; }
}