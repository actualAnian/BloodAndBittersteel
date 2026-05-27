using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace LanceSystem.CampaignBehaviors
{
    internal class AIGoBuyFoodBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, AiHourlyTick);
        }
        private void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
        {
            if (!mobileParty.IsLordParty) return;
            if (mobileParty.GetNumDaysForFoodToLast() > 6) return;
            var closestTown = SettlementHelper.FindNearestTownToMobileParty(mobileParty, MobileParty.NavigationType.All, s => { return !s.MapFaction.IsAtWarWith(mobileParty.MapFaction); });
            if (closestTown == null) return;
            var data = new AIBehaviorData(closestTown.Settlement, AiBehavior.GoToSettlement, MobileParty.NavigationType.All, false, false, false);
            if (p.TryGetBehaviorScore(data, out float num)) return;
            p.AddBehaviorScore(new ValueTuple<AIBehaviorData, float>(data, 1));
        }
        public override void SyncData(IDataStore dataStore) {}
    }
}
