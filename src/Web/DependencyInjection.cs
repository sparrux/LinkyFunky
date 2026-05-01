using FastEndpoints;
using LinkyFunky.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using LinkyFunky.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Web.Background;

namespace Web;

/// <summary>
/// Registers Web-layer dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary> 
    /// Adds Web-layer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddFastEndpoints();
        
        services.Configure<DomainOptions>(configuration.GetSection(DomainOptions.SectionName));
        
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "LinkyFunky.Auth";
                options.LoginPath = "/";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

        services.AddAuthorization();
        services.AddHostedService<CountersSyncBackgroundService>();

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
}
