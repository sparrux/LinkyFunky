using LinkyFunky.Infrastructure.Persistence;
using LinkyFunky.Infrastructure.Services;
using LinkyFunky.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkyFunky.Infrastructure;

public static class DependencyInjection
{
    const int DefaultShortCodeLength = 8;

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IShortCodeGen>(_ => new Base62ShortCodeGen(DefaultShortCodeLength));
        
        var connectionString = configuration.GetConnectionString("LinkyDb");

        services.AddDbContext<LinkyDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
