using System.Security.Claims;
using FluentResults;
using LinkyFunky.Application.Features.Users.CreateUser;
using MediatR;
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
        
        var sender = httpContext.RequestServices.GetRequiredService<IMediator>();

        var signInResult = await SignInAnonymouslyAsync(httpContext, sender);

        if (signInResult.IsFailed)
        {
            await httpContext.Response.WriteAsJsonAsync(signInResult);
            return;            
        }

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

    static async Task<Result> SignInAnonymouslyAsync(HttpContext httpContext, IMediator sender)
    {
        var userResult = await sender.Send(new CreateUserCommand());

        if (userResult.IsFailed)
            return Result.Fail(userResult.Errors);

        var user = userResult.Value;
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Anonymous"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        httpContext.User = principal;
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Result.Ok();
    }
}
