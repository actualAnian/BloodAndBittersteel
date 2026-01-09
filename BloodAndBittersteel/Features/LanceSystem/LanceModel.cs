using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using static BloodAndBittersteel.Features.LanceSystem.LancesCampaignBehavior;

namespace BloodAndBittersteel.Features.LanceSystem
{
    public abstract class LanceModel : MBGameModel<LanceModel>
    {
        public abstract int LancesFromClanTier(int clanTier);
        public abstract ExplainedNumber MaxLancesForParty(PartyBase party);
        public abstract void UpdateNotablesLanceTroops(Hero notable, NotableLanceData flattenedTroopRoster);
        public abstract ExplainedNumber GetMaxTroopsInLance(Hero notable);
        public abstract List<float> GetLanceTroopQuality(Hero notable);
        public abstract int DailyTroopsToUpgrade(Hero notable);
        public abstract int DailyTroopsGet(Hero notable);
        public abstract List<float> DefaultTroopQuality { get; }
    }
}
