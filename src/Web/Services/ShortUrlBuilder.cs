using LinkyFunky.Application.Contracts.Services;
using Microsoft.Extensions.Options;
using Web.Options;

namespace Web.Services;

/// <summary>
/// Builds public short URLs using configured domain settings.
/// </summary>
public sealed class ShortUrlBuilder : IShortUrlBuilder
{
    readonly Uri _domain;

    /// <summary>
    /// Initializes a new instance of <see cref="ShortUrlBuilder"/>.
    /// </summary>
    /// <param name="domainOptions">The configured domain options.</param>
    /// <exception cref="ArgumentException">Thrown when configured base URL is invalid.</exception>
    public ShortUrlBuilder(IOptions<DomainOptions> domainOptions)
    {
        var baseUrl = domainOptions.Value.BaseUrl;
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var domain))
        {
            throw new ArgumentException("Domain:BaseUrl must be a valid absolute URL.", nameof(domainOptions));
        }

        _domain = domain;
    }

    /// <summary>
    /// Builds a full short URL for the specified short code.
    /// </summary>
    /// <param name="shortCode">The short code segment.</param>
    /// <returns>A full absolute short URL.</returns>
    public string Build(string shortCode)
    {
        var normalizedShortCode = shortCode.TrimStart('/');
        return new Uri(_domain, normalizedShortCode).ToString();
    }
}