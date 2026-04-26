using System.Security.Cryptography;
using System.Text;
using LinkyFunky.Domain.Contracts;

namespace LinkyFunky.Infrastructure.Services;

/// <summary>
/// Generates deterministic short codes by encoding a URL hash with the Base62 alphabet.
/// </summary>
public sealed class Base62ShortCodeGen : IShortCodeGen
{
    readonly int _codeLength;
    const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Initializes a new instance of <see cref="Base62ShortCodeGen"/> with the specified code length.
    /// </summary>
    /// <param name="codeLength">The required output length for generated short codes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="codeLength"/> is less than or equal to zero.</exception>
    public Base62ShortCodeGen(int codeLength)
    {
        if (codeLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(codeLength), "Code length must be greater than zero.");
        }

        _codeLength = codeLength;
    }

    /// <summary>
    /// Generates a Base62 short code from the specified URL.
    /// </summary>
    /// <param name="longUrl">The original URL to encode.</param>
    /// <returns>A deterministic Base62 short code with a fixed length.</returns>
    /// <exception cref="ArgumentException">Thrown when the URL is null, empty, or whitespace.</exception>
    public string Generate(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("Long URL must not be null or empty.", nameof(longUrl));
        }

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(longUrl.Trim()));
        var value = BitConverter.ToUInt64(hashBytes, 0);

        return EncodeBase62(value, _codeLength);
    }

    /// <summary>
    /// Converts an unsigned integer into a fixed-length Base62 string.
    /// </summary>
    /// <param name="value">The numeric value to encode.</param>
    /// <param name="length">The required output length.</param>
    /// <returns>A Base62-encoded string padded to the requested length.</returns>
    static string EncodeBase62(ulong value, int length)
    {
        Span<char> buffer = stackalloc char[length];

        for (var index = length - 1; index >= 0; index--)
        {
            var remainder = (int)(value % 62);
            buffer[index] = Alphabet[remainder];
            value /= 62;
        }

        return new(buffer);
    }
}
