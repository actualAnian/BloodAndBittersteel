using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.CampaignStart
{
    public class BaBCampaignStartBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, OnCreationOver);
        }
        private void TeleportPlayerToStartLocation()
        {
            var startSettlementId = BaBCampaignStartConfig.GetStartSettlementFromCulture(Hero.MainHero.Culture);
            Settlement? startingSettlement = MBObjectManager.Instance.GetObject<Settlement>(startSettlementId);
            if (startingSettlement == null)
                InformationManager.DisplayMessage(new($"Error, settlement with id {startingSettlement} not found."));
            else
            {
                MobileParty.MainParty.Position = startingSettlement.GatePosition;
                if (GameStateManager.Current.ActiveState is MapState mapState)
                {
                    mapState.Handler.ResetCamera(true, true);
                    mapState.Handler.TeleportCameraToMainParty();
                }
            }
        }
        private void OnCreationOver()
        {
            TeleportPlayerToStartLocation();
        }
        public override void SyncData(IDataStore dataStore) { }
    }
}
