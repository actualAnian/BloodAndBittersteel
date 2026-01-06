using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using static BloodAndBittersteel.Features.LanceSystem.LancesCampaignBehavior;

namespace BloodAndBittersteel.Features.LanceSystem
{
    public abstract class LanceModel : MBGameModel<LanceModel>
    {
        public abstract int MaxNotableTroops(Hero notable);
        public abstract int LancesFromClanTier(int clanTier);
        public abstract ExplainedNumber MaxLancesForParty(PartyBase party);
        public abstract void UpdateNotablesLanceTroops(Hero notable, NotableLanceData flattenedTroopRoster);
        //public abstract int GetMaxTroopsInLance(Hero notable, )
    }
}
