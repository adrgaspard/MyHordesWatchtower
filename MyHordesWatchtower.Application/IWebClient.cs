using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Application
{
    public interface IWebClient
    {
        Task<string> GenerateAuthenticationCookies();

        Task<IReadOnlyList<CitizenEntry>> CollectCitizensEntries();
    }
}
