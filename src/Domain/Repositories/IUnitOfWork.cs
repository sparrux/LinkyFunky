namespace LinkyFunky.Domain.Repositories;

/// <summary>
/// Defines a unit of work contract for transactional data persistence operations.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>A transaction handle that should be disposed after use.</returns>
    Task<IDisposable> BeginTransactionAsync(CancellationToken ctk = default);

    /// <summary>
    /// Commits the current active database transaction.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    Task CommitTransactionAsync(CancellationToken ctk = default);

    /// <summary>
    /// Persists all tracked changes to the data store.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The number of affected records.</returns>
    Task<int> SaveChangesAsync(CancellationToken ctk = default);
}