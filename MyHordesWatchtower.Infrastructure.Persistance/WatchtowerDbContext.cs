using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Infrastructure.Persistance.Models;
using System.Collections.Immutable;
using System.Text.Json;

namespace MyHordesWatchtower.Infrastructure.Persistance
{
    public class WatchtowerDbContext(DbContextOptions<WatchtowerDbContext> options) : DbContext(options)
    {
        public DbSet<DbCitizenEntry> CitizenEntries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.Entity<DbCitizenEntry>()
                .ToTable("CitizenEntries");

            _ = modelBuilder.Entity<DbCitizenEntry>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<DbCitizenEntry>()
                .Property(x => x.Constructions)
                .HasConversion(
                    c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                    c => JsonSerializer.Deserialize<Dictionary<HomeConstructionType, uint>>(c, (JsonSerializerOptions?)null)!
                        .ToImmutableDictionary())
                .Metadata.SetValueComparer(new ValueComparer<IReadOnlyDictionary<HomeConstructionType, uint>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary()));

            modelBuilder.Entity<DbCitizenEntry>()
                .Property(x => x.Items)
                .HasConversion(
                    i => JsonSerializer.Serialize(i.Select(pair => new { Item = pair.Key.Name, Quantity = pair.Value })
                        .ToImmutableDictionary(tuple => tuple.Item, tuple => tuple.Quantity), (JsonSerializerOptions?)null),
                    i => JsonSerializer.Deserialize<Dictionary<string, uint>>(i, (JsonSerializerOptions?)null)!
                        .Select(pair => new { Item = new Item(pair.Key), Quantity = pair.Value })
                        .ToImmutableDictionary(tuple => tuple.Item, tuple => tuple.Quantity))
                .Metadata.SetValueComparer(new ValueComparer<IReadOnlyDictionary<Item, uint>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary()));
        }
    }
}