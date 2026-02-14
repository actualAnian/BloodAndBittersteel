using LanceSystem.Deserialization;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace LanceSystem.LanceDataClasses
{
    public class SettlementNotableLanceInfo
    {
        [SaveableProperty(1)]
        public string NotableId { get; private set; }
        [SaveableProperty(2)]
        public TroopRoster CurrentNotableLanceTroopRoster { get; private set; }
        [SaveableProperty(3)]
        public bool IsTaken { get; set; }
        [SaveableProperty(4)]
        public string? PartyLanceBelongsTo { get; set; }
        [SaveableField(5)]
        private string _lanceTemplateId;
        public ExplainedNumber CachedMaxLanceTroops { get; set; }
        public List<float> CachedMaxTroopPerTier;
        private Lance? _cachedLance;
        public Lance CurrentLance 
        {
            get
            {
                if (_cachedLance == null)
                    _cachedLance = LanceTemplateManager.Instance.GetLanceFromId(_lanceTemplateId);
                return _cachedLance;
            }
        }
        public void SetLanceTemplate(Lance template) { _lanceTemplateId = template.StringId; _cachedLance = template; }
        public IEnumerable<Lance> GetPossibleTemplates()
        {
            var settlement = MBObjectManager.Instance.GetObject<CharacterObject>(NotableId).HeroObject.HomeSettlement;
            return LanceTemplateManager.Instance.GetLances(settlement.Culture.StringId, settlement);
        }
        private static LanceTemplateOriginType GetLanceSettlementType(Settlement settlement)
        {
            if (settlement.IsTown) return LanceTemplateOriginType.Town;
            if (settlement.IsCastle) return LanceTemplateOriginType.Castle;
            if (settlement.IsVillage) return LanceTemplateOriginType.Village;
            InformationManager.DisplayMessage(new($"error getting the settlement type from {settlement.Name}"));
            return LanceTemplateOriginType.All;
        }
        public SettlementNotableLanceInfo(Hero notable, TroopRoster lanceRoster, bool isTaken, string? lanceTemplateId = null)
        {
            NotableId = notable.StringId;
            CurrentNotableLanceTroopRoster = lanceRoster;
            IsTaken = isTaken;
            CachedMaxLanceTroops = Campaign.Current.Models.LanceModel().GetMaxTroopsInLance(notable);
            if (lanceTemplateId == null)
                _lanceTemplateId = LanceTemplateManager.Instance.GetLances(notable.StringId, GetLanceSettlementType(notable.BornSettlement)).GetRandomElementInefficiently().StringId;
            else
                _lanceTemplateId = lanceTemplateId;
            CachedMaxTroopPerTier = new(Campaign.Current.Models.LanceModel().DefaultTroopQuality);
        }
    }
}