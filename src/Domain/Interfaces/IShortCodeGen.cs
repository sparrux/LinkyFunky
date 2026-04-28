using FluentResults;

namespace LinkyFunky.Domain.Interfaces;

/// <summary>
/// Defines a contract for generating short codes.
/// </summary>
public interface IShortCodeGen
{
    /// <summary>
    /// Generates a new short code value.
    /// </summary>
    /// <param name="longUrl">The original URL associated with the request context.</param>
    /// <returns>A result containing the generated short code.</returns>
    Result<string> Generate(string longUrl);
}