using FluentResults;
using LinkyFunky.Domain.Interfaces;

namespace LinkyFunky.Infrastructure.Services.ShortCodeGen;

/// <summary>
/// Generates random short codes without binding them to the source URL.
/// </summary>
public sealed class RandomShortCodeGen(int codeLength) : IShortCodeGen
{
    readonly int _codeLength = codeLength > 0
        ? codeLength
        : throw new ArgumentOutOfRangeException(nameof(codeLength), "Code length must be greater than zero.");

    const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Generates a random short code with the configured fixed length.
    /// </summary>
    /// <param name="longUrl">The original URL. The value is accepted for contract compatibility and ignored.</param>
    /// <returns>A result containing a random short code.</returns>
    public Result<string> Generate(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            return Result.Fail("Long URL must not be null or empty.");
        }

        Span<char> buffer = stackalloc char[_codeLength];

        for (var index = 0; index < buffer.Length; index++)
        {
            var alphabetIndex = Random.Shared.Next(Alphabet.Length);
            buffer[index] = Alphabet[alphabetIndex];
        }

        return Result.Ok(new string(buffer));
    }
}
