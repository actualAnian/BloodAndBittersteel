using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace LanceSystem
{
    public class LanceData
    {
        [SaveableField(1)]
        public TroopRoster LanceRoster;
        [SaveableField(2)]
        public string NotableId;
        [SaveableField(3)]
        public string SettlementStringId;
        [SaveableField(4)]
        public string Name;
        public LanceData(TroopRoster lanceRoster, string notableId, string settlementStringId, string lanceName)
        {
            LanceRoster = lanceRoster;
            NotableId = notableId;
            SettlementStringId = settlementStringId;
            Name = lanceName;
        }
        public NotableLanceData GetNotableLanceData()
        {
            return Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>().GetNotableData(NotableId);
        }
    }
}