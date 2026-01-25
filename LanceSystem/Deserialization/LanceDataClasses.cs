using System.Collections.Generic;

namespace LanceSystem.Deserialization
{
    public enum LanceTemplateOriginType
    {
        Town,
        Village,
        Castle,
        Mercenary,
        All
    }

    public record Lance(string StringId, string Name, string CultureId, LanceTemplateOriginType LanceOriginType, LanceTroopsTemplate Troops, int weight = 1);

    public record LanceTroopsTemplate(List<TroopData> TroopTypes);

    public enum LanceTroopCategory
    {
        Infantry,
        Ranged,
        Cavalry,
        HorseArcher
    }
    public record TroopData(LanceTroopCategory Category, double Likelihood, string BasicTroopId);
}
