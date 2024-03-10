using MyHordesWatchtower.Application;
using MyHordesWatchtower.Application.Repositories;
using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Host
{
    public class Application(IWebClient webClient, ICitizenEntryRepository repository)
    {
        public void StartCollection()
        {
            IReadOnlyList<CitizenEntry> entries = webClient.CollectCitizensEntries().Result;
            _ = repository.AddCitizensEntries(entries);
            _ = Console.ReadLine();
        }

        public void StartFarmThiefs()
        {
            webClient.FarmThiefs().Wait();
            _ = Console.ReadLine();
        }
    }
}
