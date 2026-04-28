using System.Security.Claims;
using FastEndpoints;
using LinkyFunky.Application.Contracts.Requests;
using LinkyFunky.Application.Contracts.Responses;
using LinkyFunky.Application.Features.Shortcuts.CreateShortcut;
using MediatR;
using Microsoft.AspNetCore.Authorization;

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
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var result = await sender.Send(new CreateShortcutCommand(userId, req.LongUrl), ctk);
        if (result.IsFailed)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsJsonAsync(result.Errors.Select(x => x.Message), ctk);
            return;
        }

        HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await HttpContext.Response.WriteAsJsonAsync(result.Value, ctk);
    }

    static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }
}
