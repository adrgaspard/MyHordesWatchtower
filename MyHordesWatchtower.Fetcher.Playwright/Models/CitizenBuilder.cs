using MyHordesWatchtower.Domain.Models.Data;
using System.Reflection;
using System.Text;

namespace MyHordesWatchtower.Fetcher.Playwright.Models
{
    public class CitizenBuilder
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

        public CitizenBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public CitizenBuilder WithPseudo(string pseudo)
        {
            _pseudo = pseudo;
            return this;
        }

        public CitizenBuilder WithProfession(Profession profession)
        {
            _profession = profession;
            return this;
        }

        public CitizenBuilder WithChaman(bool chaman)
        {
            _chaman = chaman;
            return this;
        }

        public CitizenBuilder WithLocation(CitizenLocation location)
        {
            _location = location;
            return this;
        }

        public CitizenBuilder WithStars(uint stars)
        {
            _stars = stars;
            return this;
        }

        public CitizenBuilder WithDeathStatus(DeathStatus deathStatus)
        {
            _deathStatus = deathStatus;
            return this;
        }

        public CitizenBuilder WithGossips(CitizenGossips gossips)
        {
            _gossips = gossips;
            return this;
        }

        public CitizenBuilder WithBanned(bool banned)
        {
            _banned = banned;
            return this;
        }

        public CitizenBuilder WithChargesCount(uint chargesCount)
        {
            _chargesCount = chargesCount;
            return this;
        }

        public CitizenBuilder WithHomeLevel(HomeLevel homeLevel)
        {
            _homeLevel = homeLevel;
            return this;
        }

        public CitizenBuilder WithConstructions(IReadOnlyDictionary<HomeConstructionType, uint> constructions)
        {
            _constructions = constructions;
            return this;
        }

        public CitizenBuilder WithStorage(CitizenStorage storage)
        {
            _storage = storage;
            return this;
        }

        public CitizenBuilder WithDefense(uint defense)
        {
            _defense = defense;
            return this;
        }

        public CitizenBuilder WithDecoration(uint decoration)
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
