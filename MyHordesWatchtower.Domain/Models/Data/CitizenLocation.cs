namespace MyHordesWatchtower.Domain.Models.Data
{
    public record CitizenLocation
    {
        public static readonly CitizenLocation InsideTown = new(false, 0, 0);

        public bool OutsideTown { get; private init; }

        public int X { get; private init; }

        public int Y { get; private init; }

        private CitizenLocation(bool outsideTown, int x, int y)
        {
            OutsideTown = outsideTown;
            X = x;
            Y = y;
        }

        public static CitizenLocation CreateOutside(int x, int y)
        {
            return new(true, x, y);
        }
    }
}
