using System.Collections.Generic;
using System.ComponentModel;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.BlackfyreRebellion
{
    public class BlackfyreRebellionData
    {
        [SaveableProperty(1)]
        public bool IsRebellionActive { get; set; } = false;
        [SaveableProperty(2)]
        public List<string> LoyalistVassals { get; set; } = new();
        [SaveableProperty(3)]
        public RebellionSide PlayerSide { get; set; } = RebellionSide.Neutral;
    }
    public enum RebellionSide
    {
        Loyalist,
        Rebel,
        Neutral
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

        public static RebellionCampaignBehavior Instance => Campaign.Current.GetCampaignBehavior<RebellionCampaignBehavior>();
        public override void RegisterEvents()
        {
            //CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }
        public void OnRebellionStart(RebellionSide sideChosen)
        {
            _rebellionData.IsRebellionActive = true;
            _rebellionData.PlayerSide = sideChosen;
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("BaB_BlackfyreRebellionData", ref _rebellionData);
        }
    }
}
