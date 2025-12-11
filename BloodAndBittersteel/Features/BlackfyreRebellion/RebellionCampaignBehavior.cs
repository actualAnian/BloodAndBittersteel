using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.BlackfyreRebellion
{
    // UNUSED FOR NOW, I AM ASSUMING IT WILL BE USEFUL IN THE FUTURE
    public class BlackfyreRebellionData
    {
        [SaveableProperty(1)]
        public bool IsRebellionActive { get; set; } = false;
        [SaveableProperty(2)]
        public List<string> LoyalistVassals { get; set; } = new();
    }
    public class RebellionCampaignBehavior : CampaignBehaviorBase
    {
        public BlackfyreRebellionData RebellionData => _rebellionData;
        private BlackfyreRebellionData _rebellionData;
        public RebellionCampaignBehavior()
        {
            _rebellionData = new()
            {
                LoyalistVassals = new(RebellionConfig.LoyalistVassalsAtGameStart)
            };
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
