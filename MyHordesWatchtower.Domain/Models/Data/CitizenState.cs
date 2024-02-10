namespace MyHordesWatchtower.Domain.Models.Data
{
    public record CitizenState
    {
        public DeathStatus DeathStatus { get; private init; }

        public bool Injured { get; private init; }

        public bool Infected { get; private init; }

        public bool Terrified { get; private init; }

        public bool DrugAddict { get; private init; }

        public bool Dehydrated { get; private init; }

        private CitizenState(DeathStatus deathStatus, bool injured, bool infected, bool terrified, bool drugAddict, bool dehydrated)
        {
            DeathStatus = deathStatus;
            Injured = injured;
            Infected = infected;
            Terrified = terrified;
            DrugAddict = drugAddict;
            Dehydrated = dehydrated;
        }

        public static CitizenState CreateDead(DeathStatus deathStatus)
        {
            return deathStatus == DeathStatus.StillAlive
                ? throw new ArgumentException($"This parameter can take the value {DeathStatus.StillAlive}", nameof(deathStatus))
                : new CitizenState(deathStatus, false, false, false, false, false);
        }

        public static CitizenState CreateAlive(bool injured, bool infected, bool terrified, bool drugAddict, bool dehydrated)
        {
            return new CitizenState(DeathStatus.StillAlive, injured, infected, terrified, drugAddict, dehydrated);
        }

    }
}
