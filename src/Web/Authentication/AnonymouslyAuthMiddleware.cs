using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Web.Authentication;

/// <summary>
/// Automatically signs in an anonymous user for endpoints that require authorization.
/// </summary>
public sealed class AnonymouslyAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!RequiresAuthorization(httpContext) || httpContext.User.Identity?.IsAuthenticated == true)
        {
            await next(httpContext);
            return;
        }

        await SignInAnonymouslyAsync(httpContext);
        await next(httpContext);
    }

    static bool RequiresAuthorization(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is null)
        {
            return false;
        }

        var hasAllowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null;
        if (hasAllowAnonymous)
        {
            return false;
        }

        return endpoint.Metadata.GetMetadata<IAuthorizeData>() is not null;
    }

    static Task SignInAnonymouslyAsync(HttpContext httpContext)
    {
        var testUserId = Guid.NewGuid().ToString();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUserId),
            new Claim(ClaimTypes.Name, "Anonymous")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        httpContext.User = principal;
        return httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
