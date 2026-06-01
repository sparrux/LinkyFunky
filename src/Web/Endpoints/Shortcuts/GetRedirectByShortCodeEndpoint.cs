using FastEndpoints;
using LinkyFunky.Application.Features.Shortcuts.GetShortcut;
using MediatR;
using Web.Extensions;

namespace Web.Endpoints.Shortcuts;

/// <summary>
/// Redirects a client to the original URL by shortcut code.
/// </summary>
public sealed class GetRedirectByShortCodeEndpoint(
    IMediator sender
) : EndpointWithoutRequest
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("/r/{shortCode}");
    }

    public override async Task HandleAsync(CancellationToken ctk)
    {
        var shortCode = Route<string>("shortCode");

        if (string.IsNullOrWhiteSpace(shortCode) || shortCode.Length < 3)
        {
            await HttpContext.Response.SendStatusCodeAsync(StatusCodes.Status400BadRequest, ctk);
            return;
        }
        
        var result = await sender.Send(new GetShortcutLongUrlCommand(shortCode), ctk);
        if (result.IsFailed)
        {
            await HttpContext.Response.SendResultResponseAsync(
                result, errorCode: StatusCodes.Status404NotFound, ctk: ctk);
            return;
        }

        await HttpContext.Response.SendRedirectAsync(result.Value, isPermanent: false, allowRemoteRedirects: true);
    }
}

