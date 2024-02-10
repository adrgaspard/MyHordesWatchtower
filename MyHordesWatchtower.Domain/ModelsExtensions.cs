using MyHordesWatchtower.Domain.Models.Data;

namespace MyHordesWatchtower.Domain
{
    public static class ModelsExtensions
    {
        private static bool IsAlive(this CitizenState state)
        {
            return state.DeathStatus == DeathStatus.StillAlive;
        }

        public static bool IsAlive(this Citizen citizen)
        {
            return citizen.State.IsAlive();
        }

        private static uint GetLastConnectionDay(this CitizenPresence presence, uint currentDay)
        {
            int difference = (DateTime.Now.Date - presence.LastConnectionDateTime.Date).Days;
            return currentDay <= 1 || difference > currentDay ? 1 : currentDay - (uint)difference;
        }

        public static uint GetLastConnectionDay(this Citizen citizen, uint currentDay)
        {
            return citizen.Presence.GetLastConnectionDay(currentDay);
        }

        private static bool IsProtectedAgainstRobbery(this CitizenStorage storage)
        {
            return storage[new("Chaîne de Porte + cadenas")] > 0;
        }

        private static bool IsProtectedAgainstRobbery(this CitizenConstructionRegistry registry)
        {
            return registry[HomeConstructionType.Lock] > 0 || registry.HomeLevel >= HomeLevel.FencedHouse;
        }

        public static bool IsProtectedAgainstRobbery(this Citizen citizen)
        {
            return citizen.IsAlive() && (citizen.Storage.IsProtectedAgainstRobbery() || citizen.ConstructionsRegistry.IsProtectedAgainstRobbery());
        }

        public static bool CanBeRobbed(this Citizen citizen)
        {
            return !citizen.IsAlive() || (citizen.Location.OutsideTown && !citizen.IsProtectedAgainstRobbery());
        }

        private static uint GetAPDistanceFromTown(this CitizenLocation location)
        {
            return (uint)(location.OutsideTown ? Math.Abs(location.X) + Math.Abs(location.Y) : 0);
        }

        public static uint GetAPDistanceFromTown(this Citizen citizen)
        {
            return citizen.Location.GetAPDistanceFromTown();
        }
    }
}
