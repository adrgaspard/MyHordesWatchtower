using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Domain.Models.Tools;
using MyHordesWatchtower.Infrastructure.Persistance.Models;
using System.Collections.Immutable;

namespace MyHordesWatchtower.Infrastructure.Persistance.Mappings
{
    public static class CitizenEntryMapping
    {
        public static DbCitizenEntry ToDatabaseEntity(this CitizenEntry entry)
        {
            return new()
            {
                Id = entry.Id.Value,

                BatchId = entry.Batch.Id.Value,
                BatchTimestamp = entry.Batch.Timestamp,

                HordesId = entry.Data.Identity.HordesId,
                Pseudo = entry.Data.Identity.Pseudo,

                Profession = entry.Data.Job.Profession,
                Chaman = entry.Data.Job.Chaman,

                Stars = entry.Data.Presence.Stars,
                WellUses = entry.Data.Presence.WellUses,
                LastConnectionDateTime = entry.Data.Presence.LastConnectionDateTime,

                OutsideTown = entry.Data.Location.OutsideTown,
                X = entry.Data.Location.X,
                Y = entry.Data.Location.Y,

                DeathStatus = entry.Data.State.DeathStatus,
                Injured = entry.Data.State.Injured,
                Infected = entry.Data.State.Infected,
                Terrified = entry.Data.State.Terrified,
                DrugAddict = entry.Data.State.DrugAddict,
                Dehydrated = entry.Data.State.Dehydrated,

                Banned = entry.Data.Justice.Banned,
                Charges = entry.Data.Justice.Charges,

                HomeLevel = entry.Data.ConstructionsRegistry.HomeLevel,
                Constructions = entry.Data.ConstructionsRegistry.ToImmutableDictionary(),

                Visible = entry.Data.Storage.Visible,
                Items = entry.Data.Storage.ToImmutableDictionary(),

                Defense = entry.Data.Score.Defense,
                Decoration = entry.Data.Score.Decoration,
            };
        }

        public static CitizenEntry ToBusinessEntity(this DbCitizenEntry entry)
        {
            return new(
                Id: new Id<CitizenEntry>(entry.Id),

                Batch: new Batch(
                    Id: new Id<Batch>(entry.BatchId),
                    Timestamp: entry.BatchTimestamp),

                Data: new Citizen(
                    Identity: new CitizenIdentity(
                        HordesId: entry.HordesId,
                        Pseudo: entry.Pseudo),

                    Job: new CitizenJob(
                        Profession: entry.Profession,
                        Chaman: entry.Chaman),

                    Presence: new CitizenPresence(
                        Stars: entry.Stars,
                        WellUses: entry.WellUses,
                        LastConnectionDateTime: entry.LastConnectionDateTime),

                    Location: entry.OutsideTown
                        ? CitizenLocation.CreateOutside(
                            x: entry.X,
                            y: entry.Y)
                        : CitizenLocation.InsideTown,

                    State: entry.DeathStatus == DeathStatus.StillAlive
                        ? CitizenState.CreateAlive(
                            injured: entry.Injured,
                            infected: entry.Infected,
                            terrified: entry.Terrified,
                            drugAddict: entry.DrugAddict,
                            dehydrated: entry.Dehydrated)
                        : CitizenState.CreateDead(
                            deathStatus: entry.DeathStatus),

                    Justice: new CitizenJustice(
                        Banned: entry.Banned,
                        Charges: entry.Charges),

                    ConstructionsRegistry: new CitizenConstructionRegistry(
                        constructions: entry.Constructions,
                        homeLevel: entry.HomeLevel),

                    Storage: entry.Visible
                        ? CitizenStorage.CreateVisible(
                            items: entry.Items)
                        : CitizenStorage.Invisible,

                    Score: new CitizenScore(
                        Defense: entry.Defense,
                        Decoration: entry.Decoration)
                ));
        }
    }
}
