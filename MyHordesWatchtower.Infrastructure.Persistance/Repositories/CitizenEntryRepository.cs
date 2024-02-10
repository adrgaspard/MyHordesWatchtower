using Microsoft.Extensions.Logging;
using MyHordesWatchtower.Application.Repositories;
using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Infrastructure.Persistance.Mappings;
using MyHordesWatchtower.Infrastructure.Persistance.Models;
using System.Collections.Immutable;

namespace MyHordesWatchtower.Infrastructure.Persistance.Repositories
{
    public class CitizenEntryRepository(WatchtowerDbContext dbContext, ILogger<CitizenEntryRepository> logger) : ICitizenEntryRepository
    {
        public Task AddCitizensEntries(IEnumerable<CitizenEntry> citizensEntries)
        {
            if (citizensEntries.Any())
            {
                ImmutableList<DbCitizenEntry> dbEntries = citizensEntries.Select(entries => entries.ToDatabaseEntity()).ToImmutableList();
                dbContext.CitizenEntries.AddRange(dbEntries);
                try
                {
                    logger.LogDebug("Saving {number} entries in database...", dbEntries.Count);
                    _ = dbContext.SaveChanges();
                    logger.LogDebug("{number} entries saved!", dbEntries.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "{Class}.{Method} raised an exception !", nameof(WatchtowerDbContext), nameof(WatchtowerDbContext.SaveChangesAsync));
                }
            }
            return Task.CompletedTask;
        }
    }
}
