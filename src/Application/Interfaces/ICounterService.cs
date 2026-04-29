namespace LinkyFunky.Application.Interfaces;

/// <summary>
/// Represents a named counter entry.
/// </summary>
/// <param name="Key">The counter key.</param>
/// <param name="Value">The current counter value.</param>
public sealed record Counter(string Key, int Value);

/// <summary>
/// Provides operations for working with application counters.
/// </summary>
public interface ICounterService
{
    /// <summary>
    /// Increments a counter value by key.
    /// </summary>
    /// <param name="key">The counter key.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    Task IncrementAsync(string key, CancellationToken ctk = default);

    /// <summary>
    /// Returns the current counter value by key.
    /// </summary>
    /// <param name="key">The counter key.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The current counter value.</returns>
    Task<int> GetAsync(string key, CancellationToken ctk = default);

    /// <summary>
    /// Returns all available counters.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The collection of counters.</returns>
    Task<IReadOnlyCollection<Counter>> GetAllAsync(CancellationToken ctk = default);

    /// <summary>
    /// Removes counters by keys.
    /// </summary>
    /// <param name="keys">The keys of counters to remove.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    Task RemoveAllAsync(IEnumerable<string> keys, CancellationToken ctk = default);
}