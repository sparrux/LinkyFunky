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
    const string RedisInstanceNameKey = "Redis:InstanceName";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IShortCodeGen>(_ => new RandomShortCodeGen(DefaultShortCodeLength));

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IShortcutsRepository, ShortcutsRepository>();
        
        var connectionString = configuration.GetConnectionString("linkyfunky");

        services.AddDbContext<LinkyDbContext>(options =>
        {
            options.UseNpgsql(connectionString, opt => 
                opt.MigrationsAssembly(typeof(DependencyInjection).Assembly));
        });

        services.AddRedisDistributedCache(configuration);

        return services;
    }

    /// <summary>
    /// Registers Redis-backed distributed cache services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    static void AddRedisDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetConnectionString("redis");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration;
            options.InstanceName = configuration[RedisInstanceNameKey];
        });
    }
}
