using MyHordesWatchtower.Domain.Models.Tools;

namespace MyHordesWatchtower.Domain.Models.Data
{
    public record CitizenEntry(Id<CitizenEntry> Id, Batch Batch, Citizen Data);
}
