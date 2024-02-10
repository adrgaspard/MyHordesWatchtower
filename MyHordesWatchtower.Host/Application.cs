using MyHordesWatchtower.Application;
using MyHordesWatchtower.Application.Repositories;

namespace MyHordesWatchtower.Host
{
    public class Application(IWebClient webClient, ICitizenEntryRepository repository)
    {
        public void Start()
        {
            IReadOnlyList<Domain.Models.Data.CitizenEntry> entries = webClient.CollectCitizensEntries().Result;
            _ = repository.AddCitizensEntries(entries);
            _ = Console.ReadLine();
        }
    }
}
