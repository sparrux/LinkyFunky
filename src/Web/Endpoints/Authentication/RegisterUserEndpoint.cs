using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastEndpoints;
using FluentResults;
using LinkyFunky.Application.Contracts.Responses;
using LinkyFunky.Application.Features.Users.CreateUser;
using LinkyFunky.Application.Interfaces;
using LinkyFunky.Domain.Entities;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Web.Extensions;
using Web.Metrics;

namespace Web.Endpoints.Authentication;

/// <summary>
/// Simple registration endpoint for a new users.
/// </summary>
public sealed class RegisterUserEndpoint(
    IMediator mediator, 
    UserMetrics userMetrics,
    IGuestService guestService,
    IConfiguration configuration
) : EndpointWithoutRequest
{
    readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    
    public override void Configure()
    {
        AllowAnonymous();
        Get("/reg");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            await HttpContext.Response.SendResultResponseAsync(
                Result.Fail("Already authenticated"), ctk: ct);
            return;
        }

        var createResult = await mediator.Send(new CreateUserCommand(guestService.GuestIdOrDefault), ct);

        if (createResult.IsFailed)
        {
            await HttpContext.Response.SendResultResponseAsync(createResult, ctk: ct);
            return;
        }
        
        userMetrics.NewUserCounter.Add(1);
        
        await HttpContext.Response.SendResultResponseAsync(
            Result.Ok(new TokenResponse(
                AccessToken: GenerateAccessToken(createResult.Value, configuration, _jwtSecurityTokenHandler)
            )), ctk: ct);
    }

    static string GenerateAccessToken(User user, IConfiguration configuration, JwtSecurityTokenHandler tokenHandler)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Mock User Name"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key), 
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("AccessTokenExpiryMinutes")),
            signingCredentials: credentials);

        return tokenHandler.WriteToken(token);
    }
}