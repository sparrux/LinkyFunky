using LinkyFunky.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkyFunky.Infrastructure.Persistence.EntityConfigurations;

/// <summary>
/// Configures persistence mapping for the <see cref="Shortcut"/> aggregate.
/// </summary>
public sealed class ShortcutConfiguration : IEntityTypeConfiguration<Shortcut>
{
    /// <summary>
    /// Configures the database schema for the <see cref="Shortcut"/> entity.
    /// </summary>
    /// <param name="builder">The entity builder used to configure mapping details.</param>
    public void Configure(EntityTypeBuilder<Shortcut> builder)
    {
        builder.ToTable("shortcuts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LongUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.ShortCode)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(x => x.UserId)
            .IsRequired();
        
        builder.Property(x => x.Redirects)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Shortcuts)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ShortCode)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}
