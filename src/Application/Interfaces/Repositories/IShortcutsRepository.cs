using LinkyFunky.Domain.Entities;

namespace LinkyFunky.Application.Interfaces.Repositories;

public interface IShortcutsRepository : IRepository<Shortcut>
{
    Task<IReadOnlyCollection<Shortcut>> GetAllShortcutsAsync(IEnumerable<string> shortCodes, CancellationToken ctk);
    Task<int> UpdateRedirectsAsync(Dictionary<string, int> shortCodesToIncrements, CancellationToken ctk);
}