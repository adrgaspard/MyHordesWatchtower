namespace MyHordesWatchtower.Domain.Models.Data
{
    public record Citizen(
        CitizenIdentity Identity,
        CitizenJob Job,
        CitizenPresence Presence,
        CitizenLocation Location,
        CitizenState State,
        CitizenJustice Justice,
        CitizenConstructionRegistry ConstructionsRegistry,
        CitizenStorage Storage,
        CitizenScore Score);
}
