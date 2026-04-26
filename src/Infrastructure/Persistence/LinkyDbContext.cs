using LinkyFunky.Domain.Entities;
using LinkyFunky.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LinkyFunky.Infrastructure.Persistence;

/// <summary>
/// Represents the database context for the LinkyFunky application
/// and the concrete unit of work implementation.
/// </summary>
/// <param name="options">The database context configuration options.</param>
public sealed class LinkyDbContext(DbContextOptions<LinkyDbContext> options) 
    : DbContext(options), IUnitOfWork
{
    IDbContextTransaction? currentTransaction;

    /// <summary>
    /// Gets the database set for the <see cref="User"/> entity.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the database set for the <see cref="Shortcut"/> entity.
    /// </summary>
    public DbSet<Shortcut> Shortcuts => Set<Shortcut>();

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LinkyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Begins a new database transaction for a sequence of atomic operations.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>A disposable transaction handle.</returns>
    public async Task<IDisposable> BeginTransactionAsync(CancellationToken ctk = default)
    {
        if (currentTransaction is not null)
            throw new InvalidOperationException("An active transaction already exists.");

        return currentTransaction = await Database.BeginTransactionAsync(ctk);
    }

    /// <summary>
    /// Commits the current active database transaction.
    /// </summary>
    /// <param name="ctk">The token used to cancel the operation.</param>
    public async Task CommitTransactionAsync(CancellationToken ctk = default)
    {
        if (currentTransaction is null)
            throw new InvalidOperationException("There is no active transaction to commit.");

        try
        {
            await currentTransaction.CommitAsync(ctk);
        }
        finally
        {
            await currentTransaction.DisposeAsync();
            currentTransaction = null;
        }
    }
}
