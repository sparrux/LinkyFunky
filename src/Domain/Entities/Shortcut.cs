using LinkyFunky.Domain.Common;
using LinkyFunky.Domain.Contracts;
using FluentResults;

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
    /// Gets the user who created the shortcut.
    /// </summary>
    public User User { get; private set; }

    /// <summary>
    /// Gets the user ID who created the shortcut.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Creates a new <see cref="Shortcut"/> instance for the specified URL.
    /// </summary>
    /// <param name="userId">The user identifier who owns the shortcut.</param>
    /// <param name="longUrl">The original URL to shorten.</param>
    /// <param name="shortGen">The short code generator used to create a unique code.</param>
    /// <returns>A new <see cref="Shortcut"/> instance.</returns>
    public static Result<Shortcut> Create(Guid userId, string longUrl, IShortCodeGen shortGen)
    {
        if (!IsValidLongUrl(longUrl))
        {
            return Result.Fail("The long URL must be a valid absolute HTTP or HTTPS URL.");
        }

        var shortCodeResult = shortGen.Generate(longUrl);
        if (shortCodeResult.IsFailed)
        {
            return Result.Fail(shortCodeResult.Errors);
        }

        return Result.Ok(new Shortcut
        {
            UserId = userId,
            LongUrl = longUrl,
            ShortCode = shortCodeResult.Value
        });
    }

    /// <summary>
    /// Validates whether the provided URL is an absolute HTTP or HTTPS URL.
    /// </summary>
    /// <param name="longUrl">The URL to validate.</param>
    /// <returns><c>true</c> if the URL is valid; otherwise, <c>false</c>.</returns>
    static bool IsValidLongUrl(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            return false;
        }

        if (!Uri.TryCreate(longUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }
}
