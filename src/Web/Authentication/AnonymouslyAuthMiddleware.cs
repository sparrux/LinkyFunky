using System.Security.Claims;
using LinkyFunky.Domain.Entities;
using LinkyFunky.Domain.Repositories;
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
        
        var usersRepository = httpContext.RequestServices.GetRequiredService<IUsersRepository>();

        await SignInAnonymouslyAsync(httpContext, usersRepository);
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

    static async Task SignInAnonymouslyAsync(HttpContext httpContext, IUsersRepository usersRepository)
    {
        var user = await CreateAnonymousUserAsync(usersRepository);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Anonymous"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        httpContext.User = principal;
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    static async Task<User> CreateAnonymousUserAsync(IUsersRepository usersRepository)
    {
        var user = User.Create();

        await usersRepository.AddAsync(user);
        await usersRepository.UnitOfWork.SaveChangesAsync();

        return user;
    }
}
