using System.Collections.Generic;

namespace LanceSystem.Deserialization
{
    public enum LanceTemplateOriginType
    {
        Town,
        Village,
        Castle,
        Settlement, //town + village + castle
        Mercenary,
        All
    }

    public record Lance(string StringId, string Name, string? CultureId, string? ClanId, LanceTemplateOriginType LanceOriginType, LanceTroopsTemplate TroopsTemplate, int weight = 1, string? bannerKey = null);

    public record LanceTroopsTemplate(List<TroopData> TroopTypes);

    public enum LanceTroopCategory
    {
        Infantry,
        Ranged,
        Cavalry,
        HorseArcher
    }

    public static class LanceTroopCategoryExtensions
    {
        public static LanceTroopCategory ToCategory(this int value)
        {
            return value switch
            {
                1 => LanceTroopCategory.Ranged,
                2 => LanceTroopCategory.Cavalry,
                3 => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry
            };
        }

        public static int ToInt(this LanceTroopCategory category)
        {
            return category switch
            {
                LanceTroopCategory.Infantry => 0,
                LanceTroopCategory.Ranged => 1,
                LanceTroopCategory.Cavalry => 2,
                LanceTroopCategory.HorseArcher => 3,
                _ => 0
            };
        }
    }
    public record TroopData(LanceTroopCategory Category, double Likelihood, string BasicTroopId);
}
