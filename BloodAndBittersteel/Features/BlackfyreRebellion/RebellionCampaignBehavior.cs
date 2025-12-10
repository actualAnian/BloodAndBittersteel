using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.BlackfyreRebellion
{
    public class BlackfyreRebellionData
    {
        public bool IsRebellionActive { get; set; } = false;
    }
    public class RebellionCampaignBehavior : CampaignBehaviorBase
    {
        public BlackfyreRebellionData RebellionData => _rebellionData;
        private BlackfyreRebellionData _rebellionData;
        public RebellionCampaignBehavior()
        {
            _rebellionData = new();
        }


        public override void RegisterEvents()
        {
            //CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }
        public void OnRebellionStart()
        {
            _rebellionData.IsRebellionActive = true;
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("BaB_BlackfyreRebellionData", ref _rebellionData);
        }
    }
}
