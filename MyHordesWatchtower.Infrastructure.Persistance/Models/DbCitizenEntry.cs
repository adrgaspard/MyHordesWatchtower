using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Infrastructure.Persistance.Models
{
    public class DbCitizenEntry
    {
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public DbCitizenEntry() { }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        public Guid Id { get; set; }

        #region Batch
        public Guid BatchId { get; set; }
        public DateTime BatchTimestamp { get; set; }
        #endregion

        #region Citizen

        #region Identity
        public int HordesId { get; set; }
        public string Pseudo { get; set; }
        #endregion

        #region Job
        public Profession Profession { get; set; }
        public bool Chaman { get; set; }
        #endregion

        #region Presence
        public uint Stars { get; set; }
        public uint WellUses { get; set; }
        public DateTime LastConnectionDateTime { get; set; }
        #endregion

        #region Location
        public bool OutsideTown { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        #endregion

        #region State
        public DeathStatus DeathStatus { get; set; }
        public bool Injured { get; set; }
        public bool Infected { get; set; }
        public bool Terrified { get; set; }
        public bool DrugAddict { get; set; }
        public bool Dehydrated { get; set; }
        #endregion

        #region Justice
        public bool Banned { get; set; }
        public uint Charges { get; set; }
        #endregion

        #region Constructions registry
        public HomeLevel HomeLevel { get; set; }
        public IReadOnlyDictionary<HomeConstructionType, uint> Constructions { get; set; }
        #endregion

        #region Storage
        public bool Visible { get; set; }
        public IReadOnlyDictionary<Item, uint> Items { get; set; }
        #endregion

        #region Score
        public uint Defense { get; set; }
        public uint Decoration { get; set; }
        #endregion

        #endregion
    }
}
