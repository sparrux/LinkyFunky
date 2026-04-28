using LinkyFunky.Application.Contracts.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using LinkyFunky.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Options;
using Web.Services;

namespace Web;

/// <summary>
/// Registers Web-layer dependencies.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DomainOptions>(configuration.GetSection(DomainOptions.SectionName));

        services.AddSingleton<IShortUrlBuilder>(s =>
        {
            var domainOptions = s.GetRequiredService<IOptions<DomainOptions>>();
            return new ShortUrlBuilder(domainOptions);
        });
        
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
