using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.FemaleLords
{
    internal class FemaleLordsCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.CanHeroLeadPartyEvent.AddNonSerializedListener(this, IsFemale);
        }

        private void IsFemale(Hero hero, ref bool result)
        {
            if (FemaleLordsConfig.CanLeadParties(hero.CharacterObject)) result = true;
            else result = false;
        }
        public override void SyncData(IDataStore dataStore) { }
    }
}
