namespace LinkyFunky.Domain.Contracts;

/// <summary>
/// Defines a contract for generating short codes from full URLs.
/// </summary>
public interface IShortCodeGen
{
    /// <summary>
    /// Generates a short code for the specified long URL.
    /// </summary>
    /// <param name="longUrl">The original URL that must be converted into a short code.</param>
    /// <returns>A generated short code in a URL-safe format.</returns>
    string Generate(string longUrl);
}