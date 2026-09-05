using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.CampaignBehaviors
{
    public class DisbandedLanceAI : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, AiTick);
        }

        private void AiTick(MobileParty party, PartyThinkParams p)
        {
            if (party.PartyComponent is not DisbandedLancePartyComponent pc)
                return;
            var settlementToReturnTo = MBObjectManager.Instance.GetObject<CharacterObject>(pc.NotableLanceBelongsTo).HeroObject.CurrentSettlement;

            var data = new AIBehaviorData(settlementToReturnTo, AiBehavior.GoToSettlement, MobileParty.NavigationType.All, false, false, false); 
            p.AddBehaviorScore(new ValueTuple<AIBehaviorData, float>(data, 100));
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
