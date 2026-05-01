using Infrastructure.IntegrationTests.Fixtures;
using LinkyFunky.Infrastructure.Services.Counters;

namespace Infrastructure.IntegrationTests;

/// <summary>
/// Verifies Redis-backed counter operations.
/// </summary>
public sealed class RedisCounterServiceTests(RedisCacheFixture fixture) : IClassFixture<RedisCacheFixture>
{
    [Fact]
    public async Task IncrementAsync_WhenCalledMultipleTimes_IncrementsCounterValue()
    {
        var ctk = CancellationToken.None;
        var key = $"redirects-{Guid.NewGuid():N}";
        var service = await CreateServiceAsync();

        await service.IncrementAsync(key, ctk);
        await service.IncrementAsync(key, ctk);
        await service.IncrementAsync(key, ctk);

        var value = await service.GetAsync(key, ctk);

        Assert.Equal(3, value);
    }

    [Fact]
    public async Task GetAllAsync_WhenCountersExist_ReturnsAllCountersWithValues()
    {
        var ctk = CancellationToken.None;
        var firstKey = $"first-{Guid.NewGuid():N}";
        var secondKey = $"second-{Guid.NewGuid():N}";
        var service = await CreateServiceAsync();

        await service.IncrementAsync(firstKey, ctk);
        await service.IncrementAsync(secondKey, ctk);
        await service.IncrementAsync(secondKey, ctk);

        var counters = await service.GetAllAsync(ctk);

        Assert.Contains(counters, x => x.Key == firstKey && x.Value == 1);
        Assert.Contains(counters, x => x.Key == secondKey && x.Value == 2);
    }

    [Fact]
    public async Task RemoveAllAsync_WhenKeysExist_RemovesSpecifiedCounters()
    {
        var ctk = CancellationToken.None;
        var removableKey = $"removable-{Guid.NewGuid():N}";
        var keepKey = $"keep-{Guid.NewGuid():N}";
        var service = await CreateServiceAsync();

        await service.IncrementAsync(removableKey, ctk);
        await service.IncrementAsync(keepKey, ctk);

        await service.RemoveAllAsync([removableKey], ctk);

        var removedValue = await service.GetAsync(removableKey, ctk);
        var keptValue = await service.GetAsync(keepKey, ctk);

        Assert.Equal(0, removedValue);
        Assert.Equal(1, keptValue);
    }

    async Task<RedisCounterService> CreateServiceAsync()
    {
        var connection = await fixture.CreateConnection();
        return new(connection);
    }
}