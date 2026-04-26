using LinkyFunky.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkyFunky.Infrastructure.Persistence;

/// <summary>
/// Represents the database context for the LinkyFunky application.
/// </summary>
public sealed class LinkyDbContext(DbContextOptions<LinkyDbContext> options) : DbContext(options)
{
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
}
