using LinkyFunky.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Infrastructure.IntegrationTests;

/// <summary>
/// Provides a PostgreSQL Testcontainer and creates isolated EF Core contexts for integration tests.
/// </summary>
public sealed class PostgresDatabaseFixture : IAsyncLifetime
{
    readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("linky_funky_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    /// <summary>
    /// Creates a new database context connected to the running PostgreSQL test container.
    /// </summary>
    public LinkyDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<LinkyDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new(options);
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var dbContext = CreateContext();
        await dbContext.Database.EnsureCreatedAsync();
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
