using System.Security.Claims;
using LinkyFunky.Application.Interfaces;

namespace Web.Middleware;

/// <summary>
/// Applies per-user daily Redis-backed quotas to shortcut creation and redirect endpoints.
/// </summary>
/// <param name="next">The next middleware delegate.</param>
public sealed class UserDailyRateLimitMiddleware(RequestDelegate next)
{
    static readonly HashSet<string> ReservedRedirectSegments =
    [
        "health",
        "alive",
        "scalar",
        "openapi",
    ];

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
        if (result.Allowed)
        {
            if (result.Limit > 0)
            {
                httpContext.Response.Headers.Append("RateLimit-Limit", result.Limit.ToString());
                httpContext.Response.Headers.Append("RateLimit-Remaining", result.Remaining.ToString());
            }

            await next(httpContext);
            return;
        }

        var retrySeconds = (int)Math.Ceiling((result.RetryAfter ?? TimeSpan.Zero).TotalSeconds);
        httpContext.Response.Headers.Append("Retry-After", Math.Max(retrySeconds, 1).ToString());
        if (result.Limit > 0)
        {
            httpContext.Response.Headers.Append("RateLimit-Limit", result.Limit.ToString());
            httpContext.Response.Headers.Append("RateLimit-Remaining", "0");
        }
        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await httpContext.Response.WriteAsJsonAsync(
            new { error = "Rate limit exceeded", window = "utc_day" },
            httpContext.RequestAborted);
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

    static bool IsCreateShortcutRequest(HttpRequest request) =>
        HttpMethods.IsPost(request.Method)
        && request.Path.Equals("/shortcuts", StringComparison.OrdinalIgnoreCase);

    static bool IsRedirectShortcutRequest(HttpRequest request)
    {
        if (!HttpMethods.IsGet(request.Method))
            return false;

        var path = request.Path;
        if (!path.HasValue || path == "/")
            return false;

        var segments = path.Value.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 1)
            return false;

        return !ReservedRedirectSegments.Contains(segments[0], StringComparer.OrdinalIgnoreCase);
    }
}
