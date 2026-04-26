using LinkyFunky.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkyFunky.Infrastructure.Persistence;

public sealed class LinkyDbContext(DbContextOptions<LinkyDbContext> options) : DbContext(options)
{
    public DbSet<Shortcut> Shortcuts => Set<Shortcut>();

    public DbSet<User> Users => Set<User>();
}
