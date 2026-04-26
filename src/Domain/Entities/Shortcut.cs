using LinkyFunky.Domain.Common;
using LinkyFunky.Domain.Contracts;

namespace LinkyFunky.Domain.Entities;

/// <summary>
/// Represents a shortened link aggregate that stores the original URL and its unique short code.
/// </summary>
public sealed class Shortcut : Entity
{
    private Shortcut() { }
    
    /// <summary>
    /// Gets the original URL provided by a user for shortening.
    /// </summary>
    public string LongUrl { get; private set; }
    /// <summary>
    /// Gets the unique short code used for redirecting to the original URL.
    /// </summary>
    public string ShortCode { get; private set; }

    /// <summary>
    /// Creates a new <see cref="Shortcut"/> instance for the specified URL.
    /// </summary>
    /// <param name="longUrl">The original URL to shorten.</param>
    /// <param name="shortGen">The short code generator used to create a unique code.</param>
    /// <returns>A new <see cref="Shortcut"/> instance.</returns>
    public static Shortcut Create(string longUrl, IShortCodeGen shortGen)
    {
        return new()
        {
            LongUrl = longUrl,
            ShortCode = shortGen.Generate(longUrl)
        };
    }
}
