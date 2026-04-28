using FastEndpoints;
using LinkyFunky.Application.Features.Shortcuts.GetShortcut;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Web.Endpoints.Shortcuts;

/// <summary>
/// Redirects a client to the original URL by shortcut code.
/// </summary>
[Authorize]
public sealed class GetRedirectByShortCodeEndpoint(IMediator sender)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/{shortCode}");
    }

    public override async Task HandleAsync(CancellationToken ctk)
    {
        var shortCode = Route<string>("shortCode");

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            await HttpContext.Response.SendStatusCodeAsync(StatusCodes.Status400BadRequest, ctk);
            return;
        }
        
        var result = await sender.Send(new GetShortcutLongUrlCommand(shortCode), ctk);
        if (result.IsFailed)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await HttpContext.Response.WriteAsJsonAsync(result.Errors.Select(x => x.Message), ctk);
            return;
        }

        await HttpContext.Response.SendRedirectAsync(result.Value, isPermanent: false, allowRemoteRedirects: true);
    }
}

