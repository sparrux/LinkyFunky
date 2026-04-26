using LinkyFunky.Domain.Common;

namespace LinkyFunky.Domain.Entities;

/// <summary>
/// Represents an anonymous user identity used for ownership and rate-limiting in the system.
/// </summary>
public sealed class User : Entity
{
    private User() { }

    /// <summary>
    /// Gets the shortcuts created by the user.
    /// </summary>
    public IReadOnlyCollection<Shortcut> Shortcuts { get; private set; }

    /// <summary>
    /// Creates a new <see cref="User"/> instance.
    /// </summary>
    /// <returns>A new <see cref="User"/> instance.</returns>
    public static User Create()
    {
        return new();
    }
}