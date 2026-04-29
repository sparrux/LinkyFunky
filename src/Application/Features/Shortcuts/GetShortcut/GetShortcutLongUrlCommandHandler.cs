using FluentResults;
using LinkyFunky.Application.Defaults;
using LinkyFunky.Application.Interfaces.Cache;
using LinkyFunky.Application.Interfaces.Repositories;
using MediatR;

namespace LinkyFunky.Application.Features.Shortcuts.GetShortcut;

/// <summary>
/// Handles long URL lookup by short code.
/// </summary>
public sealed class GetShortcutLongUrlCommandHandler(
    IShortcutsRepository shortcutsRepository,
    ICacheService cacheService
) : IRequestHandler<GetShortcutLongUrlCommand, Result<string>>
{
    /// <summary>
    /// Returns the original long URL for a shortcut code.
    /// </summary>
    /// <param name="request">The lookup command containing the short code.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>A result containing the found long URL.</returns>
    public async Task<Result<string>> Handle(GetShortcutLongUrlCommand request, CancellationToken ctk)
    {
        var normalizedCode = request.ShortCode.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCode))
            return Result.Fail("Short code is required.");

        if (await cacheService.GetAsync<string>(CacheDefaults.LongUrlKey(normalizedCode), ctk) is { } longUrlCache)
        {
            UpdateCounter();
            return Result.Ok(longUrlCache);
        }

        var longUrl = (await shortcutsRepository.SelectToListAsync(
            shortcutsRepository.QueryableSet
                .Where(x => x.ShortCode == normalizedCode), 
            q => q.LongUrl, ctk)).FirstOrDefault();

        if (longUrl is null)
            return Result.Fail("Shortcut was not found.");
        
        await cacheService.SetAsync(CacheDefaults.LongUrlKey(longUrl), longUrl, ctk);

        UpdateCounter();
        return Result.Ok(longUrl);
    }
    
    void UpdateCounter() { }
}
