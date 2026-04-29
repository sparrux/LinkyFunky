using LinkyFunky.Application.Interfaces.Repositories;
using LinkyFunky.Domain.Entities;

namespace LinkyFunky.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides persistence operations for <see cref="User"/> aggregates.
/// </summary>
/// <param name="dbContext">The database context used by the repository.</param>
public sealed class UsersRepository(LinkyDbContext dbContext) 
    : Repository<User>(dbContext), IUsersRepository;