using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.MapEvents;
using BloodAndBittersteel.Features.LanceSystem.Deserialization;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace BloodAndBittersteel.Features.LanceSystem
{
    public class NotableLanceData
    {
        public string NotableId { get; private set; }
        public TroopRoster CurrentNotableLanceTroopRoster { get; private set; }
        public bool IsTaken { get; set; }
        public string? PartyLanceBelongsTo { get; set; }
        private string _lanceTemplateId;
        public ExplainedNumber CachedMaxLanceTroops { get; set; }
        public List<float> CachedMaxTroopPerTier;
        public LanceTroopsTemplate CurrentTroopTemplate 
        {
            get
            {
                return LanceTemplateManager.Instance.GetLanceFromId(_lanceTemplateId).Troops;
            }
        }
        public void SetLanceTemplate(string templateId) { _lanceTemplateId = templateId; }
        private static LanceTemplateSettlementType GetLanceSettlementType(Settlement settlement)
        {
            if (settlement.IsTown) return LanceTemplateSettlementType.Town;
            if (settlement.IsCastle) return LanceTemplateSettlementType.Castle;
            if (settlement.IsVillage) return LanceTemplateSettlementType.Village;
            InformationManager.DisplayMessage(new($"error getting the settlement type from {settlement.Name}"));
            return LanceTemplateSettlementType.All;
        }

        public NotableLanceData(Hero notable, TroopRoster lanceRoster, bool isTaken, string? lanceTemplateId = null)
        {
            NotableId = notable.StringId;
            CurrentNotableLanceTroopRoster = lanceRoster;
            IsTaken = isTaken;
            CachedMaxLanceTroops = Campaign.Current.Models.LanceModel().GetMaxTroopsInLance(notable);
            if (lanceTemplateId == null)
                _lanceTemplateId = LanceTemplateManager.Instance.GetLances(notable.StringId, GetLanceSettlementType(notable.CurrentSettlement)).GetRandomElementInefficiently().StringId;
            else
                _lanceTemplateId = lanceTemplateId;
            CachedMaxTroopPerTier = new(Campaign.Current.Models.LanceModel().DefaultTroopQuality);
        }
    }
    public class LanceData
    {
        public TroopRoster LanceRoster;
        public string NotableId;
        public string SettlementStringId;
        public LanceData(TroopRoster lanceRoster, string notableId, string settlementStringId)
        {
            LanceRoster = lanceRoster;
            NotableId = notableId;
            SettlementStringId = settlementStringId;
        }
    }

    public class LancesCampaignBehavior : CampaignBehaviorBase
    {
        readonly Dictionary<string, List<LanceData>> _activeLancesForParties = new();

        readonly Dictionary<string, NotableLanceData> _notablesLance = new();
        public List<LanceData> Lances = new();
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, AddNotables);
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, UpdateNotablesLance);
            CampaignEvents.OnPlayerPartyKnockedOrKilledTroopEvent.AddNonSerializedListener(this, OnPlayerPartyKilledTroop);
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, (MapEvent me) => 
            {
                UpdateLanceTroops(PartyBase.MainParty);
            });
            CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, UpdateMaxNotableTroops);
        }

        private void UpdateMaxNotableTroops(Town town, Building building, int arg3)
        {
            foreach(var notable in town.Settlement.Notables)
            {
                _notablesLance.TryGetValue(notable.StringId, out var lanceData);
                if (lanceData == null) return;
                lanceData.CachedMaxTroopPerTier = Campaign.Current.Models.LanceModel().GetLanceTroopQuality(notable);
            }
        }

        public void UpdateLanceTroops(PartyBase party)
        {
            var lances = GetOrCreateLances(party);
            if (lances.Count == 0)
                return;

            var memberRoster = party.MemberRoster;
            foreach (var troop in memberRoster.GetTroopRoster())
            {
                int excess = LanceUtils.CalculateNumberOfTroopsToRemove(troop, lances);
                if (excess <= 0)
                    continue;
                LanceUtils.RemoveTroopsRandomlyFromLances(troop, excess, lances);
            }
            RemoveLancesIfEmpty(party);
        }

        private void OnMapEventEnded(MapEvent mapEvent)
        {
            foreach (var party in mapEvent.InvolvedParties) 
                UpdateLanceTroops(party);
        }
        private void RemoveLancesIfEmpty(PartyBase party)
        {
            var lances = GetOrCreateLances(party);
            lances.RemoveAll(lance => lance.LanceRoster.TotalManCount == 0);
        }


        private void OnPlayerPartyKilledTroop(CharacterObject character)
        {
            foreach (var partyLances in _activeLancesForParties.Values)
            {
                foreach (var lance in partyLances)
                {
                    lance.LanceRoster.RemoveTroop(character);
                }
            }
        }

        private void UpdateNotablesLance(Settlement settlement)
        {   
            try
            {
                foreach (var notable in settlement.Notables)
                {
                    var lanceModel = Campaign.Current.Models.LanceModel();
                    lanceModel.UpdateNotablesLanceTroops(notable, _notablesLance[notable.StringId]);
                }
            }
            catch(Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Error in LancesCampaignBehavior.UpdateNotablesLance: {ex.Message}", new Color(1,0,0)));
            }
        }

        private void AddNotables(CampaignGameStarter starter, int i)
        {
            foreach(var notabable in Hero.AllAliveHeroes.Where(h => h.IsNotable))
            {
                _notablesLance[notabable.StringId] = new(notabable, TroopRoster.CreateDummyTroopRoster(), false);
            }
        }

        private void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_request_lance", "hero_main_options", "lance_1", "I wish to recruit a lance",
                () => { return CharacterObject.OneToOneConversationCharacter.IsHero && CharacterObject.OneToOneConversationCharacter.HeroObject.IsNotable; }, 
                null, 100, null, null);
            starter.AddDialogLine("lance_1", "lance_1", "lance_main_options", "{INFO}", new ConversationSentence.OnConditionDelegate(GenerateNotableTroopsText), null, 100, null);
            starter.AddDialogLine("lance_take", "lance_main_options", "hero_main_options", "I will take them", null, () => { GiveTroopsToParty(PartyBase.MainParty, CharacterObject.OneToOneConversationCharacter.HeroObject); }, 100, null);
            starter.AddDialogLine("lance_no", "lance_main_options", "hero_main_options", "I will pass", null, null, 100, null);
            //starter.AddDialogLine("lance_1", line.InputId, line.GoToLineId, line.Text, line.Condition, line.Consequence, 100, null);
        }
        private bool GenerateNotableTroopsText()
        {
            //text = "I currently have no troops available for a lance. Please check back later.";
            var notable = CharacterObject.OneToOneConversationCharacter.HeroObject;
            var troops = _notablesLance[notable.StringId].CurrentNotableLanceTroopRoster;
            string text = "I can offer you the following troops for your lance:\n";
            foreach (var element in troops.GetTroopRoster())
                text += $"- {element.Number} x {element.Character.Name}\n";
            GameTexts.SetVariable("INFO", text);
            return true;
        }

        private void GiveTroopsToParty(PartyBase party, Hero notable)
        {
            var lanceData = _notablesLance[notable.StringId];
            lanceData.IsTaken = true;
            lanceData.PartyLanceBelongsTo = party.Id;
            var notableTroops = lanceData.CurrentNotableLanceTroopRoster;
            var partyLanceTroops = TroopRoster.CreateDummyTroopRoster();
            party.MemberRoster.Add(notableTroops);
            partyLanceTroops.Add(notableTroops);
            var lancesList = GetOrCreateLances(party);
            lancesList.Add(new LanceData(partyLanceTroops, notable.StringId, notable.CurrentSettlement.StringId));
            notableTroops.Clear();
        }

        public List<LanceData> GetOrCreateLances(PartyBase party)
        {
            if (!_activeLancesForParties.TryGetValue(party.Id, out var lances))
            {
                lances = new List<LanceData>();
                _activeLancesForParties[party.Id] = lances;
            }
            return lances;
        }
        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
