using MyHordesWatchtower.Domain.Models.Data;
using MyHordesWatchtower.Infrastructure.WebClient.Models;

namespace MyHordesWatchtower.Infrastructure.WebClient.Tools
{
    public static class ConversionExtensions
    {
        public static int ConvertToId(this string str)
        {
            return int.TryParse(str, out int id) ? id : 0;
        }

        public static Profession ConvertToProfession(this string str)
        {
            str = str.Trim().ToLower();
            return str switch
            {
                "mort !" => Profession.Dead,
                "habitant" => Profession.Inhabitant,
                "fouineur" => Profession.Digger,
                "éclaireur" => Profession.Pathfinder,
                "gardien" => Profession.Guard,
                "ermite" => Profession.Hermit,
                "apprivoiseur" => Profession.Tamer,
                "technicien" => Profession.Technician,
                _ => Profession.Unknown,
            };
        }

        public static CitizenLocation ConvertToLocation(this string str)
        {
            str = str.Trim()
                .Replace("-", "")
                .Replace("[", "")
                .Replace("]", "");
            if (string.IsNullOrWhiteSpace(str))
            {
                return CitizenLocation.InsideTown;
            }
            string[] coords = str.Split(',', 2);
            return coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y)
                ? CitizenLocation.CreateOutside(x, y)
                : CitizenLocation.InsideTown;
        }

        public static DeathStatus ConvertToDeathStatus(this string? str)
        {
            if (str is null)
            {
                return DeathStatus.StillAlive;
            }
            str = str.ToLower().Replace("( voir son âme)", "").Trim();
            return str.Contains("lacéré(e)… dévoré(e)… pendant l'attaque de la nuit")
                ? DeathStatus.TownNightAttack
                : str.Contains("disparu(e) dans l'outre-monde pendant la nuit")
                ? DeathStatus.DesertNightAttack
                : str.Contains("déshydratation")
                ? DeathStatus.Dehydration
                : str.Contains("infection")
                ? DeathStatus.Infection
                : str.Contains("drogue")
                ? DeathStatus.DrugShortage
                : str.Contains("empoisonnement")
                ? DeathStatus.Poison
                : str.Contains("ingestion de cyanure")
                ? DeathStatus.Cyanide
                : str.Contains("goule affamée")
                ? DeathStatus.GhoulStarvation
                : str.Contains("dévoré(e) par une goule")
                ? DeathStatus.GhoulDevour
                : str.Contains("goule abattue")
                ? DeathStatus.GhoulSlaughter
                : str.Contains("pendaison")
                ? DeathStatus.Gallows
                : str.Contains("cage à viande")
                ? DeathStatus.MeatCage
                : str.Contains("âme torturée")
                ? DeathStatus.TorturedSoul
                : str.Contains("mort par l'atome") ? DeathStatus.Vaporization : DeathStatus.Unknown;
        }

        public static CitizenGossips ConvertToCitizenGossips(this IEnumerable<string> gossips)
        {
            CitizenGossips result = new();
            foreach (string? gossip in gossips.Select(g => g.ToLower().Trim()))
            {
                if (gossip.Contains("a été aperçu pour la dernière fois"))
                {
                    result.LastConnectionDateTime = gossip.ConvertToLastConnectionDate();
                    continue;
                }
                if (gossip.Contains("a été vu au puits prenant"))
                {
                    string[] splittedGossip = gossip.Replace("a été vu au puits prenant", "").Trim().Split(' ');
                    if (splittedGossip.Length > 0 && uint.TryParse(splittedGossip[0], out uint wellUses))
                    {
                        result.WellUses = wellUses;
                    }
                    continue;
                }
                if (gossip.Contains("a été gravement blessé"))
                {
                    result.Injured = true;
                    continue;
                }
                if (gossip.Contains("certains parlent d'une infection"))
                {
                    result.Infected = true;
                    continue;
                }
                if (gossip.Contains("tiendrait des propos incohérents"))
                {
                    result.Terrified = true;
                    continue;
                }
                if (gossip.Contains("serait dans un grave état de dépendance"))
                {
                    result.DrugAddict = true;
                    continue;
                }
                if (gossip.Contains("aurait mendié pour de l'eau dans tout le village"))
                {
                    result.Dehydrated = true;
                    continue;
                }
            }
            return result;
        }

        private static DateTime ConvertToLastConnectionDate(this string gossip)
        {
            if (gossip.Contains("un instant"))
            {
                return DateTime.Now;
            }
            string[] splittedGossip = gossip.Split(' ');
            if (splittedGossip.Length <= 2)
            {
                return DateTime.MinValue;
            }
            if (gossip.Contains("minute(s)"))
            {
                return int.TryParse(splittedGossip[^2], out int minutes) ? DateTime.Now.AddMinutes(-minutes) : DateTime.MinValue;
            }
            if (gossip.Contains("heure(s)"))
            {
                return int.TryParse(splittedGossip[^2], out int hours) ? DateTime.Now.AddHours(-hours) : DateTime.MinValue;
            }
            if (!TimeOnly.TryParse(splittedGossip[^1].Replace(".", ""), out TimeOnly time))
            {
                return DateTime.MinValue;
            }
            DateTime dateTime = gossip.Contains("ce matin") || gossip.Contains("cet après-midi") ? DateTime.Now
                : gossip.Contains("avant-hier") ? DateTime.Now.AddDays(-2)
                : gossip.Contains("hier") ? DateTime.Now.AddDays(-1)
                : DateTime.TryParse(splittedGossip[^3], out DateTime specificDate) ? specificDate
                : DateTime.MinValue;
            return dateTime != DateTime.MinValue ? new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, time.Hour, time.Minute, dateTime.Second) : dateTime;
        }

        public static HomeLevel ConvertToHomeLevel(this string str)
        {
            str = str.Trim();
            return int.TryParse(str, out int level) ? (HomeLevel)level : HomeLevel.Unknown;
        }

        public static HomeConstructionType ConvertToHomeConstructionType(this string str)
        {
            return str.Trim().ToLower() switch
            {
                "alarme rudimentaire" => HomeConstructionType.Alarm,
                "cave-laboratoire" => HomeConstructionType.Laboratory,
                "clôture" => HomeConstructionType.Fence,
                "coin sieste" => HomeConstructionType.NapArea,
                "cuisine" => HomeConstructionType.Kitchen,
                "gros rideau" => HomeConstructionType.Curtain,
                "rangements" => HomeConstructionType.Storage,
                "renforts" => HomeConstructionType.Reinforcements,
                "verrou" => HomeConstructionType.Lock,
                _ => HomeConstructionType.Unknown
            };
        }

        public static uint ConvertToDefense(this string? str)
        {
            return uint.TryParse((str ?? "").Trim().Split(' ')[0], out uint defense) ? defense : 0;
        }

        public static uint ConvertToDecoration(this string? str)
        {
            return uint.TryParse((str ?? "").Trim(), out uint decoration) ? decoration : 0;
        }
    }
}
