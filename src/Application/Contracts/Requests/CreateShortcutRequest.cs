using System.Text.Json.Serialization;

namespace LinkyFunky.Application.Contracts.Requests;

/// <summary>
/// Represents a request to create a shortened URL.
/// </summary>
public sealed class CreateShortcutRequest
{
    /// <summary>
    /// Gets the original URL to be shortened.
    /// </summary>
    public required string LongUrl { get; init; }
}
