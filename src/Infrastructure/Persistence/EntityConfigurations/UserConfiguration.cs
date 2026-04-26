using LinkyFunky.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkyFunky.Infrastructure.Persistence.EntityConfigurations;

/// <summary>
/// Configures persistence mapping for the <see cref="User"/> entity.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the database schema for the <see cref="User"/> entity.
    /// </summary>
    /// <param name="builder">The entity builder used to configure mapping details.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);
    }
}
