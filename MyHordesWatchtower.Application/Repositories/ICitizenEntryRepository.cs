using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Application.Repositories
{
    public interface ICitizenEntryRepository
    {
        Task AddCitizensEntries(IEnumerable<CitizenEntry> citizensEntries);
    }
}
