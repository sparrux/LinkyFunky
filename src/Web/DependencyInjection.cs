using System.Text;
using FastEndpoints;
using LinkyFunky.Application.Interfaces;
using LinkyFunky.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web.Background;
using Web.Metrics;
using Web.Services;

namespace Web;

/// <summary>
/// Registers Web-layer dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary> 
    /// Adds Web-layer services to the service collection.
    /// </summary>
    /// <param name="builder">The hosting builder.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddWebServices(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var services = builder.Services;
        
        services.AddOpenApi();
        services.AddFastEndpoints();
        
        builder.AddConfiguredAuthentication();
        
        services.AddAuthorization();
        services.AddHostedService<CountersSyncBackgroundService>();

        builder.AddOpenTelemetry();

        services.AddHttpContextAccessor();
        services.AddScoped<IGuestService, GuestService>();

        return services;
    }

    /// <summary>
    /// Applies pending database migrations automatically.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The asynchronous operation result.</returns>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LinkyDbContext>();
        
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () => await dbContext.Database.MigrateAsync());
    }

    /// <summary>
    /// Applies configured JWT Bearer authentication for this API project.
    /// </summary>
    /// <param name="builder">Web API application builder.</param>
    /// <returns>Web API application builder.</returns>
    static IHostApplicationBuilder AddConfiguredAuthentication(this IHostApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return builder;
    }

    /// <summary>
    /// Adds configured open telemetry with metrics for current API project.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    static IHostApplicationBuilder AddOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter(UserMetrics.InstrumentsSourceName));

        builder.Services.AddSingleton<UserMetrics>();
        
        return builder;
    }
}
