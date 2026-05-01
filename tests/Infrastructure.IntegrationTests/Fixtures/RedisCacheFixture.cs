using StackExchange.Redis;
using Testcontainers.Redis;

namespace Infrastructure.IntegrationTests.Fixtures;

/// <summary>
/// Provides a Redis Testcontainer and creates a RedisCounterService instance.
/// </summary>
public sealed class RedisCacheFixture : IAsyncLifetime
{
    readonly RedisContainer _container = new RedisBuilder("redis:7.4-alpine")
        .Build();

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public Task<ConnectionMultiplexer> CreateConnection()
    {
        return ConnectionMultiplexer.ConnectAsync(_container.GetConnectionString());
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}