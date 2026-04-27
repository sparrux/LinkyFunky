namespace LinkyFunky.Application.Contracts.Responses;

/// <summary>
/// Represents a shortcut payload returned from application use cases.
/// </summary>
/// <param name="ShortUrl">The short URL that redirects to the original long URL.</param>
public sealed record ShortcutResponse(
    string ShortUrl
);
