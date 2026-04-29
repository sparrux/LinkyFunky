using FluentResults;
using LinkyFunky.Application.Defaults;
using LinkyFunky.Application.Interfaces.Cache;
using MediatR;

namespace LinkyFunky.Application.Features.Shortcuts.GetShortcut;

/// <summary>
/// Requests a long URL by the provided short code.
/// </summary>
/// <param name="ShortCode">The short code of the shortcut.</param>
public sealed record GetShortcutLongUrlCommand(
    string ShortCode
) : IRequest<Result<string>>, ICacheable
{
    public TimeSpan? Expiry => CacheDefaults.ShortcutsExpiry;
}
