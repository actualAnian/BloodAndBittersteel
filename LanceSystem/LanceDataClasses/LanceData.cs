using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.SaveSystem;

namespace LanceSystem.LanceDataClasses
{
    public enum LanceType
    {
        NotableLance,
        MercenaryLance
    }
    public abstract class LanceData
    {
        [SaveableField(1)]
        public TroopRoster LanceRoster;
        [SaveableField(2)]
        public string SettlementStringId;
        [SaveableField(3)]
        public string Name;

        public abstract int TotalManCount { get; }
        protected LanceData(TroopRoster lanceRoster, string settlementStringId, string name)
        {
            LanceRoster = lanceRoster;
            SettlementStringId = settlementStringId;
            Name = name;
        }
    }
}