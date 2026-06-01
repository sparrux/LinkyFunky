using FastEndpoints;
using LinkyFunky.Application.Interfaces;
using Web.Metrics;

namespace Web.Middlewares;

/// <summary>
/// Applies information to the guest cookie that is not authenticated in the system.
/// </summary>
/// <param name="next"></param>
sealed class GuestMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext ctx, IGuestService guestService, UserMetrics userMetrics)
    {
        var endpoint = ctx.GetEndpoint();
        
        var isEndpoint = endpoint?.Metadata.GetMetadata<EndpointDefinition>() is not null;

        if (!isEndpoint)
        {
            return next(ctx);
        }

        var user = ctx.User.Identity;
        var isAuthenticated = user?.IsAuthenticated ?? false;
        
        if (!isAuthenticated && !guestService.IsGuest)
        {
            guestService.SetGuest();
            userMetrics.GuestUserCounter.Add(1);
        }

        if (isAuthenticated)
        {
            guestService.DeleteGuest();
        }

        return next(ctx);
    }
}