namespace LinkyFunky.Domain.Common;

/// <summary>
/// Represents a base entity with a unique identifier.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; set; }
}