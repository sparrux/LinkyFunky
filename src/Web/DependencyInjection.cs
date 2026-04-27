using Microsoft.AspNetCore.Authentication.Cookies;
using LinkyFunky.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Web;

/// <summary>
/// Registers Web-layer dependencies.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "LinkyFunky.Auth";
                options.LoginPath = "/";
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Applies pending database migrations automatically in Development environment.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>The asynchronous operation result.</returns>
    public static async Task ApplyMigrationsInDevelopmentAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LinkyDbContext>();
        
        await RunMigrationAsync(dbContext);
    }
    
    static async Task RunMigrationAsync(LinkyDbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync();
        });
    }
}
