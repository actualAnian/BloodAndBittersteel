using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Settlements;

namespace LanceSystem.Deserialization
{
    public enum LanceTemplateOriginType
    {
        Town,
        Village,
        Castle,
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
