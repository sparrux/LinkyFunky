using LinkyFunky.Domain.Common;
using LinkyFunky.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LinkyFunky.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides a generic Entity Framework implementation of repository operations.
/// </summary>
/// <typeparam name="TEntity">The aggregate entity type handled by this repository.</typeparam>
/// <param name="dbContext">The database context used to access persistence storage.</param>
public abstract class Repository<TEntity>(LinkyDbContext dbContext) : IRepository<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Gets the unit of work associated with the current repository context.
    /// </summary>
    public IUnitOfWork UnitOfWork => dbContext;

    /// <summary>
    /// Gets the queryable source for the current entity set.
    /// </summary>
    public IQueryable<TEntity> QueryableSet => dbContext.Set<TEntity>();

    public async Task AddAsync(TEntity entity, CancellationToken ctk = default)
    {
        await dbContext.AddAsync(entity, ctk);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken ctk = default)
    {
        dbContext.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken ctk = default)
    {
        dbContext.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Guid id, CancellationToken ctk = default)
    {
        return QueryableSet.AnyAsync(x => x.Id == id, ctk);
    }

    public Task<bool> AnyAsync(IQueryable<TEntity> query, CancellationToken ctk = default)
    {
        return query.AnyAsync(ctk);
    }

    public Task<TEntity> FirstAsync(IQueryable<TEntity> query, Guid id, CancellationToken ctk = default)
    {
        return query.FirstAsync(x => x.Id == id, ctk);
    }

    public Task<TEntity> FirstAsync(IQueryable<TEntity> query, CancellationToken ctk = default)
    {
        return query.FirstAsync(ctk);
    }

    public Task<TEntity?> FirstOrDefaultAsync(IQueryable<TEntity> query, Guid id, CancellationToken ctk = default)
    {
        return query.FirstOrDefaultAsync(x => x.Id == id, ctk);
    }

    public Task<TEntity?> FirstOrDefaultAsync(IQueryable<TEntity> query, CancellationToken ctk = default)
    {
        return query.FirstOrDefaultAsync(ctk);
    }

    public Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query, CancellationToken ctk = default)
    {
        return query.ToListAsync(ctk);
    }
}