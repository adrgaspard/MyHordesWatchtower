namespace MyHordesWatchtower.Fetcher.Playwright.Models
{
    public class CitizenGossips
    {
        public DateTime LastConnectionDateTime { get; set; }

        public uint WellUses { get; set; }

        public bool Injured { get; set; }

        public bool Infected { get; set; }

        public bool Terrified { get; set; }

        public bool DrugAddict { get; set; }

        public bool Dehydrated { get; set; }

        public CitizenGossips()
        {
            LastConnectionDateTime = DateTime.MinValue;
            WellUses = 0;
            Injured = false;
            Infected = false;
            Terrified = false;
            DrugAddict = false;
            Dehydrated = false;
        }

        public override string ToString()
        {
            return typeof(CitizenGossips).Name + " { " + $"{nameof(LastConnectionDateTime)} = {LastConnectionDateTime}, {nameof(WellUses)} = {WellUses}, {nameof(Injured)} = {Injured}, {nameof(Infected)} = {Infected}, {nameof(Terrified)} = {Terrified}, {nameof(DrugAddict)} = {DrugAddict}, {nameof(Dehydrated)} = {Dehydrated}" + " }";
        }
    }
}
