namespace LinkyFunky.Application.Contracts.Responses;

/// <summary>
/// Represents a shortcut payload returned from application use cases.
/// </summary>
public sealed record ShortcutResponse(
    string ShortCode
);
