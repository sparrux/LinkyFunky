using LinkyFunky.Infrastructure.Persistence;
using LinkyFunky.Infrastructure.Services;
using LinkyFunky.Domain.Contracts;
using LinkyFunky.Domain.Repositories;
using LinkyFunky.Infrastructure.Persistence.Repositories;
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

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IShortcutsRepository, ShortcutsRepository>();
        
        var connectionString = configuration.GetConnectionString("linkyfunky");

        services.AddDbContext<LinkyDbContext>(options =>
        {
            options.UseNpgsql(connectionString, opt => 
                opt.MigrationsAssembly(typeof(DependencyInjection).Assembly));
        });

        return services;
    }
}
