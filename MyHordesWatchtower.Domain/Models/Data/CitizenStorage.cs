using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace MyHordesWatchtower.Domain.Models.Data
{
    public record CitizenStorage : IReadOnlyDictionary<Item, uint>
    {
        public static readonly CitizenStorage Invisible = new(new Dictionary<Item, uint>().ToImmutableDictionary(), false);

        public bool Visible { get; private init; }

        private readonly IReadOnlyDictionary<Item, uint> Items;

        private CitizenStorage(IReadOnlyDictionary<Item, uint> items, bool visible)
        {
            Items = items.ToImmutableDictionary();
            Visible = visible;
        }

        public uint this[Item key] => Items.TryGetValue(key, out uint quantity) ? quantity : 0;

        public IEnumerable<Item> Keys => Items.Keys;

        public IEnumerable<uint> Values => Items.Values;

        public int Count => Items.Count;

        public static CitizenStorage CreateVisible(IReadOnlyDictionary<Item, uint> items)
        {
            return new CitizenStorage(items, true);
        }

        public bool ContainsKey(Item key)
        {
            return Items.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<Item, uint>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public bool TryGetValue(Item key, [MaybeNullWhen(false)] out uint value)
        {
            return Items.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
