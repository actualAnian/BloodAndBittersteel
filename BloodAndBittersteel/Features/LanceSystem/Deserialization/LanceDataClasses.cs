using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Settlements;

namespace BloodAndBittersteel.Features.LanceSystem.Deserialization
{
    public enum LanceTemplateSettlementType
    {
        Town,
        Village,
        Castle,
        All
    }

    public record Lance(string StringId, string CultureId, LanceTemplateSettlementType SettlementType, LanceTroopsTemplate Troops);

    public record LanceTroopsTemplate(TroopData MeleeTroop, TroopData RangedTroop, TroopData CavalryTroop, TroopData HorseArcherTroop);

    public record TroopData(double Likelihood, string BasicTroopId);
}
