namespace LinkyFunky.Application.Interfaces;

/// <summary>
/// Defines a contract for building short URLs.
/// </summary>
public interface IShortUrlBuilder
{
    /// <summary>
    /// Builds a short URL for a given short code.
    /// </summary>
    /// <param name="shortCode">The short code to build the URL for.</param>
    /// <returns>The built short URL.</returns>
    string Build(string shortCode);
}