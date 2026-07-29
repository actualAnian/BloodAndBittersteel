using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.FemaleLords
{
    internal class FemaleLordsCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.CanHeroLeadPartyEvent.AddNonSerializedListener(this, IsFemale);
        }

        private void IsFemale(Hero t1, ref bool t2)
        {
            if (FemaleLordsConfig.CanLeadParties(t1.CharacterObject)) t2 = true;
            t2 = false;
        }
        public override void SyncData(IDataStore dataStore) { }
    }
}
