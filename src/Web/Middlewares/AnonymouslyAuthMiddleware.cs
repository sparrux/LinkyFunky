using System.Security.Claims;
using FluentResults;
using LinkyFunky.Application.Features.Users.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Web.Extensions;

namespace Web.Middlewares;

/// <summary>
/// Automatically signs in an anonymous user for endpoints that require authorization.
/// </summary>
public sealed class AnonymouslyAuthMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Invokes the middleware to automatically sign in an anonymous user for endpoints that require authorization.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
            await httpContext.Response.SendResultResponseAsync(signInResult);
            return;            
        }

        await next(httpContext);
    }

    /// <summary>
    /// Determines if the current endpoint requires authorization.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>True if the endpoint requires authorization, false otherwise.</returns>
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

    /// <summary>
    /// Signs in an anonymous user for the current HTTP context.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="sender">The mediator instance.</param>
    /// <returns>A result containing the signed in user.</returns>
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
