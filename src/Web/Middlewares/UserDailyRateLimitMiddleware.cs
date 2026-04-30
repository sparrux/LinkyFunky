using System.Security.Claims;
using FluentResults;
using LinkyFunky.Application.Interfaces;
using Web.Defaults;
using Web.Extensions;

namespace Web.Middlewares;

/// <summary>
/// Applies per-user daily Redis-backed quotas to shortcut creation and redirect endpoints.
/// </summary>
/// <param name="next">The next middleware delegate.</param>
public sealed class UserDailyRateLimitMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Invokes the middleware to enforce daily limits when the request maps to a rate-limited route.
    /// </summary>
    public async Task InvokeAsync(HttpContext httpContext, IUserDailyRateLimitService rateLimiter)
    {
        if (!TryResolveRateLimitKind(httpContext.Request, out var kind))
        {
            await next(httpContext);
            return;
        }

        if (!TryGetUserId(httpContext.User, out var userId))
        {
            await next(httpContext);
            return;
        }

        var result = await rateLimiter.TryAcquireAsync(userId, kind, httpContext.RequestAborted);
        
        await HandleRateLimitAsync(result, httpContext);
    }

    static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out userId);
    }

    static bool TryResolveRateLimitKind(HttpRequest request, out UserRateLimitKind kind)
    {
        kind = default;
        if (IsCreateShortcutRequest(request))
        {
            kind = UserRateLimitKind.CreateShortcut;
            return true;
        }

        if (IsRedirectShortcutRequest(request))
        {
            kind = UserRateLimitKind.RedirectShortcut;
            return true;
        }

        return false;
    }

    async Task HandleRateLimitAsync(RateLimitAcquireResult result, HttpContext httpContext)
    {
        httpContext.Response.Headers.Append(AppHeaderNames.RateLimitLimit, result.Limit.ToString());
        var shouldWriteRemainingHeader = result.Limit > 0;

        if (result.Allowed)
        {
            if (shouldWriteRemainingHeader)
                httpContext.Response.Headers.Append(AppHeaderNames.RateLimitRemaining, result.Remaining.ToString());

            await next(httpContext);
            return;
        }

        var retrySeconds = (int)Math.Ceiling((result.RetryAfter ?? TimeSpan.Zero).TotalSeconds);
        httpContext.Response.Headers.Append(AppHeaderNames.RetryAfter, Math.Max(retrySeconds, 1).ToString());

        if (shouldWriteRemainingHeader)
            httpContext.Response.Headers.Append(AppHeaderNames.RateLimitRemaining, "0");

        await httpContext.Response.SendResultResponseAsync(
            Result.Fail("Rate limit exceeded"),
            errorCode: StatusCodes.Status429TooManyRequests,
            ctk: httpContext.RequestAborted);
    }

    static bool IsCreateShortcutRequest(HttpRequest request) =>
        HttpMethods.IsPost(request.Method)
        && request.Path.Equals("/shortcuts", StringComparison.OrdinalIgnoreCase);

    static bool IsRedirectShortcutRequest(HttpRequest request)
    {
        if (!HttpMethods.IsGet(request.Method))
            return false;

        var startsWithRedirectPrefix = request.Path.StartsWithSegments("/r", out var remainingPath);
        return startsWithRedirectPrefix && remainingPath.HasValue && remainingPath.Value.StartsWith('/');
    }
}
