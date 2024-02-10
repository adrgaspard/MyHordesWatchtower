using MyHordesWatchtower.Domain.Models.Data;
using System.Reflection;
using System.Text;

namespace MyHordesWatchtower.Infrastructure.WebClient.Models
{
    public class CitizenEntryBuilder
    {
        private int _id;
        private string? _pseudo;
        private Profession? _profession;
        private bool? _chaman;
        private CitizenLocation? _location;
        private uint? _stars;
        private DeathStatus? _deathStatus;
        private CitizenGossips? _gossips;
        private bool? _banned;
        private uint? _chargesCount;
        private HomeLevel? _homeLevel;
        private IReadOnlyDictionary<HomeConstructionType, uint>? _constructions;
        private CitizenStorage? _storage;
        private uint? _defense;
        private uint? _decoration;

        public CitizenEntryBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public CitizenEntryBuilder WithPseudo(string pseudo)
        {
            _pseudo = pseudo;
            return this;
        }

        public CitizenEntryBuilder WithProfession(Profession profession)
        {
            _profession = profession;
            return this;
        }

        public CitizenEntryBuilder WithChaman(bool chaman)
        {
            _chaman = chaman;
            return this;
        }

        public CitizenEntryBuilder WithLocation(CitizenLocation location)
        {
            _location = location;
            return this;
        }

        public CitizenEntryBuilder WithStars(uint stars)
        {
            _stars = stars;
            return this;
        }

        public CitizenEntryBuilder WithDeathStatus(DeathStatus deathStatus)
        {
            _deathStatus = deathStatus;
            return this;
        }

        public CitizenEntryBuilder WithGossips(CitizenGossips gossips)
        {
            _gossips = gossips;
            return this;
        }

        public CitizenEntryBuilder WithBanned(bool banned)
        {
            _banned = banned;
            return this;
        }

        public CitizenEntryBuilder WithChargesCount(uint chargesCount)
        {
            _chargesCount = chargesCount;
            return this;
        }

        public CitizenEntryBuilder WithHomeLevel(HomeLevel homeLevel)
        {
            _homeLevel = homeLevel;
            return this;
        }

        public CitizenEntryBuilder WithConstructions(IReadOnlyDictionary<HomeConstructionType, uint> constructions)
        {
            _constructions = constructions;
            return this;
        }

        public CitizenEntryBuilder WithStorage(CitizenStorage storage)
        {
            _storage = storage;
            return this;
        }

        public CitizenEntryBuilder WithDefense(uint defense)
        {
            _defense = defense;
            return this;
        }

        public CitizenEntryBuilder WithDecoration(uint decoration)
        {
            _decoration = decoration;
            return this;
        }

        public Citizen Build()
        {
            return new(
                new(_id, _pseudo!),
                new(_profession!.Value, _chaman!.Value),
                new(_stars!.Value, _gossips!.WellUses, _gossips!.LastConnectionDateTime),
                _location!,
                _profession! != Profession.Dead
                    ? CitizenState.CreateAlive(_gossips!.Injured, _gossips!.Infected, _gossips!.Terrified, _gossips!.DrugAddict, _gossips!.Dehydrated)
                    : CitizenState.CreateDead(_deathStatus!.Value),
                new(_banned!.Value, _chargesCount!.Value),
                new(_constructions!, _homeLevel!.Value),
                _storage!,
                new(_defense!.Value, _decoration!.Value));
        }

        public override string? ToString()
        {
            return GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(info => (info.Name, Value: info.GetValue(this) ?? "(null)"))
                .Aggregate(new StringBuilder(), (sb, pair) => sb.AppendLine($"{pair.Name}: {(pair.Value is IReadOnlyDictionary<HomeConstructionType, uint> constructions
                    ? "[" + string.Join(", ", constructions.Select(pair => $"({pair.Key}, {pair.Value})")) + "]"
                    : pair.Value
                )}"), sb => sb.ToString());
        }
    }
}
