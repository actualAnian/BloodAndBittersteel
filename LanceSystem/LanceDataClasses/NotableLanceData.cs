using LanceSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.SaveSystem;

namespace LanceSystem.LanceDataClasses
{
    public class NotableLanceData : LanceData
    {
        [SaveableField(2)]
        public string NotableId;
        public NotableLanceData(TroopRoster lanceRoster, string notableId, string settlementStringId, string lanceName)
            : base(lanceRoster, settlementStringId, lanceName)
        {
            NotableId = notableId;
        }
        public override int TotalManCount => GetSettlementNotableLanceInfo().CachedMaxLanceTroops.RoundedResultNumber;
        public SettlementNotableLanceInfo GetSettlementNotableLanceInfo()
        {
            return Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>().GetNotableData(NotableId);
        }
    }
}