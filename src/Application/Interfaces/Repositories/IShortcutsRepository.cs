using LinkyFunky.Domain.Entities;

namespace LinkyFunky.Application.Interfaces.Repositories;

/// <summary>
/// Defines persistence operations for shortcuts.
/// </summary>
public interface IShortcutsRepository : IRepository<Shortcut>
{
    /// <summary>
    /// Gets all shortcuts by their short codes.
    /// </summary>
    /// <param name="shortCodes">The short codes of the shortcuts to get.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>A collection of shortcuts.</returns>
    Task<IReadOnlyCollection<Shortcut>> GetAllShortcutsAsync(IEnumerable<string> shortCodes, CancellationToken ctk);

    /// <summary>
    /// Updates the redirects count for multiple shortcuts by their short codes.
    /// </summary>
    /// <param name="shortCodesToIncrements">A dictionary of short codes and their corresponding increments.</param>
    /// <param name="ctk">The token used to cancel the operation.</param>
    /// <returns>The number of affected records.</returns>
    Task<int> UpdateRedirectsAsync(Dictionary<string, int> shortCodesToIncrements, CancellationToken ctk);
}