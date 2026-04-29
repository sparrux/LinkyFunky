using FluentResults;
using LinkyFunky.Application.Contracts.Responses;
using LinkyFunky.Application.Defaults;
using LinkyFunky.Application.Interfaces.Cache;
using MediatR;

namespace LinkyFunky.Application.Features.Shortcuts.CreateShortcut;

/// <summary>
/// Creates a new shortcut for the specified user and URL.
/// </summary>
/// <param name="UserId">The user identifier who owns the shortcut.</param>
/// <param name="LongUrl">The original URL to shorten.</param>
public sealed record CreateShortcutCommand(
    Guid UserId, 
    string LongUrl
) : IRequest<Result<ShortcutResponse>>, ICacheable
{
    public TimeSpan? Expiry => CacheDefaults.ShortcutsExpiry;
}
