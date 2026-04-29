using LinkyFunky.Application.Interfaces.Repositories;
using LinkyFunky.Domain.Entities;

namespace LinkyFunky.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides persistence operations for <see cref="Shortcut"/> aggregates.
/// </summary>
/// <param name="dbContext">The database context used by the repository.</param>
public sealed class ShortcutsRepository(LinkyDbContext dbContext) 
    : Repository<Shortcut>(dbContext), IShortcutsRepository;