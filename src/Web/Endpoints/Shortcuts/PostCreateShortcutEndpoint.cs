using System.Security.Claims;
using FastEndpoints;
using LinkyFunky.Application.Contracts.Requests;
using LinkyFunky.Application.Contracts.Responses;
using LinkyFunky.Application.Features.Shortcuts.CreateShortcut;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Web.Extensions;

namespace Web.Endpoints.Shortcuts;

/// <summary>
/// Creates a new shortcut for the current authorized user.
/// </summary>
[Authorize]
public sealed class PostCreateShortcutEndpoint(IMediator sender) : Endpoint<CreateShortcutRequest, ShortcutResponse>
{
    public override void Configure()
    {
        Post("/shortcuts");
    }

    public override async Task HandleAsync(CreateShortcutRequest req, CancellationToken ctk)
    {
        if (!TryGetUserId(HttpContext.User, out var userId))
        {
            await HttpContext.Response.SendStatusCodeAsync(StatusCodes.Status401Unauthorized, ctk);
            return;
        }

        var result = await sender.Send(new CreateShortcutCommand(userId, req.LongUrl), ctk);
        if (result.IsFailed)
        {
            await HttpContext.Response.SendResultResponseAsync(result, ctk: ctk);
            return;
        }

        await HttpContext.Response.SendResultResponseAsync(result, ctk: ctk);
    }

    static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }
}
