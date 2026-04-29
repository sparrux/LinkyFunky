using LinkyFunky.Application.Interfaces.Cache;
using LinkyFunky.Application.Interfaces.Repositories;
using LinkyFunky.Application.Interfaces;
using LinkyFunky.Domain.Interfaces;
using LinkyFunky.Infrastructure.Persistence;
using LinkyFunky.Infrastructure.Services.Cache;
using LinkyFunky.Infrastructure.Services.Counters;
using LinkyFunky.Infrastructure.Persistence.Repositories;
using LinkyFunky.Infrastructure.Services.ShortCodeGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace LinkyFunky.Infrastructure;

public static class DependencyInjection
{
    const int DefaultShortCodeLength = 8;
    const string RedisInstanceNameKey = "Redis:InstanceName";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddServices();
        services.AddDatabase(configuration);
        services.AddRedisDistributedCache(configuration);

        return services;
    }
    
    static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IShortcutsRepository, ShortcutsRepository>();
        services.AddScoped<ICacheService, RedisDistributedCache>();
        services.AddScoped<ICounterService, RedisCounterService>();
        services.AddSingleton<IShortCodeGen>(_ => new RandomShortCodeGen(DefaultShortCodeLength));
    }
    
    static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("linkyfunky");
        
        services.AddDbContext<LinkyDbContext>(options =>
        {
            options.UseNpgsql(connectionString, opt => 
                opt.MigrationsAssembly(typeof(DependencyInjection).Assembly));
        });
    }

    /// <summary>
    /// Registers Redis-backed distributed cache services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    static void AddRedisDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetConnectionString("redis");
        if (string.IsNullOrWhiteSpace(redisConfiguration))
            throw new InvalidOperationException("Connection string 'redis' is not configured.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration;
            options.InstanceName = configuration[RedisInstanceNameKey];
        });

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration));
    }
}
