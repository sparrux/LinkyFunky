using FluentResults;
using LinkyFunky.Application.Contracts.Responses;
using LinkyFunky.Application.Interfaces;
using LinkyFunky.Domain.Entities;
using LinkyFunky.Domain.Interfaces;
using MediatR;

namespace LinkyFunky.Application.Features.Shortcuts.CreateShortcut;

/// <summary>
/// Handles shortcut creation requests.
/// </summary>
public sealed class CreateShortcutCommandHandler(
    IShortcutsRepository shortcutsRepository,
    IShortCodeGen shortCodeGen,
    IShortUrlBuilder builder
) : IRequestHandler<CreateShortcutCommand, Result<ShortcutResponse>>
{
    /// <summary>
    /// Creates and persists a new shortcut for an existing user.
    /// </summary>
    /// <param name="request">The create shortcut command.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>A result containing the created shortcut response.</returns>
    public async Task<Result<ShortcutResponse>> Handle(CreateShortcutCommand request, CancellationToken ctk)
    {
        var shortcutResult = Shortcut.Create(request.UserId, request.LongUrl, shortCodeGen);
        if (shortcutResult.IsFailed)
            return Result.Fail(shortcutResult.Errors);

        var shortcut = shortcutResult.Value;
        await shortcutsRepository.AddAsync(shortcut, ctk);
        await shortcutsRepository.UnitOfWork.SaveChangesAsync(ctk);

        var response = new ShortcutResponse(builder.Build(shortcut.ShortCode), shortcut.ShortCode);
        return Result.Ok(response);
    }
}
