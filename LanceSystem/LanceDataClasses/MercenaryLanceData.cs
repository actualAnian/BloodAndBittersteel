using LanceSystem.CampaignBehaviors;
using LanceSystem.Deserialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.SaveSystem;

namespace LanceSystem.LanceDataClasses
{
    public class MercenaryLanceData : LanceData
    {
        [SaveableField(1)]
        public CampaignTime ContractEndTime;
        [SaveableField(2)]
        public int Size;
        [SaveableField(3)]
        readonly string _templateId;
        [SaveableField(4)]
        public string PartyBelongedToStringId;
        [SaveableField(5)]
        public bool HasMissedPayment;
        public MercenaryLanceData(string settlementId, TroopRoster mercenaryTroops, string name, int size, Lance template, string partyBelongedTo) : base(mercenaryTroops, settlementId, name)
        {
            _templateId = template.StringId;
            _cachedTemplate = template;
            Size = size;
            PartyBelongedToStringId = partyBelongedTo;
            ContractEndTime = CampaignTime.DaysFromNow(MercenaryLancesInTavernsCampaignBehavior.ContractLengthInDays);
            HasMissedPayment = false;
        }
        private Lance? _cachedTemplate;
        public Lance TroopsTemplate => _cachedTemplate ??= LanceTemplateManager.Instance.GetLanceFromId(_templateId);

        public override int TotalManCount => Size;
    }
}
