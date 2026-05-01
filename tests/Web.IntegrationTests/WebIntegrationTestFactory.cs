using LinkyFunky.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Web.IntegrationTests;

/// <summary>
/// Creates an isolated Web host configured with PostgreSQL and Redis Testcontainers.
/// </summary>
public sealed class WebIntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("linky_funky_web_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    readonly RedisContainer _redisContainer = new RedisBuilder("redis:7.4-alpine")
        .Build();

    /// <summary>
    /// Creates a database context connected to the PostgreSQL test container.
    /// </summary>
    public LinkyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LinkyDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        return new LinkyDbContext(options);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:linkyfunky"] = _postgresContainer.GetConnectionString(),
                ["ConnectionStrings:redis"] = _redisContainer.GetConnectionString(),
                ["Domain:BaseUrl"] = "https://lf.test/",
                ["RateLimits:CreateShortcutPerUtcDay"] = "1000",
                ["RateLimits:RedirectShortcutPerUtcDay"] = "1000"
            });
        });
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await _redisContainer.StartAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings__linkyfunky", _postgresContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings__redis", _redisContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("RateLimits__CreateShortcutPerUtcDay", "1000");
        Environment.SetEnvironmentVariable("RateLimits__RedirectShortcutPerUtcDay", "1000");

        _ = CreateClient();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LinkyDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    /// <inheritdoc />
    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        Environment.SetEnvironmentVariable("ConnectionStrings__linkyfunky", null);
        Environment.SetEnvironmentVariable("ConnectionStrings__redis", null);
        Environment.SetEnvironmentVariable("RateLimits__CreateShortcutPerUtcDay", null);
        Environment.SetEnvironmentVariable("RateLimits__RedirectShortcutPerUtcDay", null);
        await _postgresContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }
}
