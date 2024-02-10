using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace MyHordesWatchtower.Domain.Models.Data
{
    public record CitizenConstructionRegistry : IReadOnlyDictionary<HomeConstructionType, uint>
    {
        private readonly IReadOnlyDictionary<HomeConstructionType, uint> Constructions;

        public HomeLevel HomeLevel { get; private init; }

        public CitizenConstructionRegistry(IReadOnlyDictionary<HomeConstructionType, uint> constructions, HomeLevel homeLevel)
        {
            Constructions = constructions.ToImmutableDictionary();
            HomeLevel = homeLevel;
        }

        public uint this[HomeConstructionType key] => Constructions.TryGetValue(key, out uint level) ? level : 0;

        public IEnumerable<HomeConstructionType> Keys => Constructions.Keys;

        public IEnumerable<uint> Values => Constructions.Values;

        public int Count => Constructions.Count;

        public bool ContainsKey(HomeConstructionType key)
        {
            return Constructions.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<HomeConstructionType, uint>> GetEnumerator()
        {
            return Constructions.GetEnumerator();
        }

        public bool TryGetValue(HomeConstructionType key, [MaybeNullWhen(false)] out uint value)
        {
            return Constructions.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Constructions.GetEnumerator();
        }
    }
}
