using LanceSystem.LanceDataClasses;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace LanceSystem.Models
{
    public abstract class LanceModel : MBGameModel<LanceModel>
    {
        public abstract int LancesFromClanTier(int clanTier);
        public abstract ExplainedNumber MaxLancesForParty(PartyBase party);
        public abstract void UpdateNotablesLanceTroops(Hero notable, SettlementNotableLanceInfo flattenedTroopRoster);
        public abstract ExplainedNumber GetMaxTroopsInLance(Hero notable);
        public abstract List<float> GetLanceTroopQuality(Hero notable);
        public abstract int DailyTroopsToUpgrade(Hero notable);
        public abstract int DailyTroopsGet(Hero notable);
        public abstract List<float> DefaultTroopQuality { get; }
        public abstract ExplainedNumber GetRetinueSizeLimit(PartyBase party);
    }
}
