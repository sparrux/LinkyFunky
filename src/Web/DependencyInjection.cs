using Microsoft.AspNetCore.Authentication.Cookies;

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
}
