using System.Text;
using LinkyFunky.Application.Interfaces.Repositories;
using LinkyFunky.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LinkyFunky.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides persistence operations for <see cref="Shortcut"/> aggregates.
/// </summary>
/// <param name="dbContext">The database context used by the repository.</param>
public sealed class ShortcutsRepository(LinkyDbContext dbContext) 
    : Repository<Shortcut>(dbContext), IShortcutsRepository
{
    public async Task<IReadOnlyCollection<Shortcut>> GetAllShortcutsAsync(IEnumerable<string> shortCodes, CancellationToken ctk)
    {
        return await ToListAsync(
            QueryableSet.Where(s => shortCodes.Contains(s.ShortCode)), ctk);
    }

    public Task<int> UpdateRedirectsAsync(Dictionary<string, int> shortCodesToIncrements, CancellationToken ctk)
    {
        if (shortCodesToIncrements.Count == 0)
            return Task.FromResult(0);
        
        var sqlParams = new List<object>(shortCodesToIncrements.Count * 2);
        var valuesSql = new StringBuilder();
        
        var index = 0;
        foreach (var pair in shortCodesToIncrements)
        {
            if (index > 0)
                valuesSql.Append(", ");
            valuesSql.Append($"(@code{index}, @delta{index})");
            sqlParams.Add(new NpgsqlParameter($"code{index}", pair.Key));
            sqlParams.Add(new NpgsqlParameter($"delta{index}", pair.Value));
            index++;
        }
        
        var sql = $"""
                   UPDATE shortcuts AS s
                   SET "Redirects" = s."Redirects" + v.delta
                   FROM (VALUES {valuesSql}) AS v(short_code, delta)
                   WHERE s."ShortCode" = v.short_code;
                   """;

        return dbContext.Database.ExecuteSqlRawAsync(sql, sqlParams, ctk);
    }
}