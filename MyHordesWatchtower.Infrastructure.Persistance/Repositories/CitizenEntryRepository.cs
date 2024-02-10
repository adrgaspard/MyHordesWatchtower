using MyHordesWatchtower.Application.Repositories;
using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Infrastructure.Persistance.Mappings;

namespace MyHordesWatchtower.Infrastructure.Persistance.Repositories
{
    public class CitizenEntryRepository(WatchtowerDbContext dbContext) : ICitizenEntryRepository
    {
        public async Task AddCitizensEntries(IEnumerable<CitizenEntry> citizensEntries)
        {
            await dbContext.AddRangeAsync(citizensEntries.Select(entries => entries.ToDatabaseEntity()));
            await dbContext.SaveChangesAsync();
        }
    }
}
