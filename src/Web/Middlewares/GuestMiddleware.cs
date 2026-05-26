using FastEndpoints;
using LinkyFunky.Application.Interfaces;
using Web.Metrics;

namespace Web.Middlewares;

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