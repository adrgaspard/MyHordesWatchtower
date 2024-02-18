using Microsoft.Extensions.Configuration;
using MyHordesWatchtower.Application;
using MyHordesWatchtower.Application.Repositories;
using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Host
{
    public class Application(IWebClient webClient, ICitizenEntryRepository repository)
    {
        public void Start()
        {
            IReadOnlyList<CitizenEntry> entries = webClient.CollectCitizensEntries().Result;
            _ = repository.AddCitizensEntries(entries);
            _ = Console.ReadLine();
        }
    }
}
