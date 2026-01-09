using BloodAndBittersteel.Features.LanceSystem.Deserialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using static TaleWorlds.MountAndBlade.MovementOrder;

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
        private LanceTroopsTemplate? _cachedTemplate;
        public TextObject Name
        {
            get
            {
                var notable = MBObjectManager.Instance.GetObject<Hero>(NotableId);
                return LanceConfig.GetLanceName(notable, notable.HomeSettlement);
            }
        }
        public LanceTroopsTemplate CurrentTroopTemplate 
        {
            get
            {
                if (_cachedTemplate == null)
                    _cachedTemplate = LanceTemplateManager.Instance.GetLanceFromId(_lanceTemplateId).Troops;
                return _cachedTemplate;
            }
        }
        public void SetLanceTemplate(string templateId) { _lanceTemplateId = templateId; _cachedTemplate = null; }
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
                _lanceTemplateId = LanceTemplateManager.Instance.GetLances(notable.StringId, GetLanceSettlementType(notable.BornSettlement)).GetRandomElementInefficiently().StringId;
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
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, UpdateNotablesLance);
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, (MapEvent me) => 
            {
                UpdateLanceTroops(PartyBase.MainParty);
            });
            CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, UpdateMaxNotableTroops);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
            CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, OnHeroPrisonerTaken);
            CampaignEvents.HeroCreated.AddNonSerializedListener(this, OnHeroCreated);
        }

        private void OnHeroCreated(Hero hero, bool arg2)
        {
            var list = new List<Occupation>
            {
                Occupation.GangLeader,
                Occupation.Artisan,
                Occupation.Merchant,
                Occupation.RuralNotable,
                Occupation.Headman
            };
            if (!list.Contains(hero.Occupation)) return;
            _notablesLance[hero.StringId] = new(hero, TroopRoster.CreateDummyTroopRoster(), false);
        }

        private void OnHeroPrisonerTaken(PartyBase capturerParty, Hero hero)
        {
            if (hero == Hero.MainHero)
                DestroyAllPartyLances(hero.PartyBelongedTo.Party);
        }
        private void DestroyAllPartyLances(PartyBase party)
        {
            if (!HasLances(party)) return;
            List<LanceData> list = GetOrCreateLances(party);
            for (int i = list.Count - 1; i >= 0; i--)
                RemoveLance(list, list[i]);

        }
        private void OnMobilePartyDestroyed(MobileParty party, PartyBase destroyerParty)
        {
            DestroyAllPartyLances(party.Party);
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
        private void RemoveLance(List<LanceData> allLances, LanceData lance)
        {
            _notablesLance[lance.NotableId].IsTaken = false;
            _notablesLance[lance.NotableId].PartyLanceBelongsTo = null;
            allLances.Remove(lance);
        }
        private void RemoveLancesIfEmpty(PartyBase party)
        {
            var lances = GetOrCreateLances(party);
            for (int i = lances.Count - 1; i >= 0; i--)
            {
                LanceData? lance = lances[i];
                if (lance.LanceRoster.TotalManCount != 0) continue;
                RemoveLance(lances, lance);
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
                InformationManager.DisplayMessage(new InformationMessage($"Error in LancesCampaignBehavior.UpdateNotablesLance: {ex.Message}", new TaleWorlds.Library.Color(1,0,0)));
            }
        }
        private void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_request_lance", "hero_main_options", "lance_1", "I wish to recruit a lance",
                () => { return CharacterObject.OneToOneConversationCharacter.IsHero && CharacterObject.OneToOneConversationCharacter.HeroObject.IsNotable; }, 
                null, 100, null, null);
            starter.AddDialogLine("lance_no", "lance_1", "hero_main_options", "{REFUSAL_TEXT}", new ConversationSentence.OnConditionDelegate(LanceTextVariation.ChooseTextVariationWhenNotableRefusesToEnlistLance), null);
            starter.AddDialogLine("lance_1", "lance_1", "lance_main_options", "{INFO}", new ConversationSentence.OnConditionDelegate(GenerateNotableTroopsText), null, 100, null);
            starter.AddPlayerLine("lance_take", "lance_main_options", "hero_main_options", "{TAKE_TEXT}", new ConversationSentence.OnConditionDelegate(LanceTextVariation.ChooseTextVariationWhenEnlistingLance), () => { GiveTroopsToParty(PartyBase.MainParty, CharacterObject.OneToOneConversationCharacter.HeroObject); }, 100, null);
            starter.AddPlayerLine("lance_no", "lance_main_options", "hero_main_options", "{REFUSAL_TEXT}", new ConversationSentence.OnConditionDelegate(LanceTextVariation.ChooseTextVariantWhenNotTakingLance), null, 100, null);
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
            lancesList.Add(new LanceData(partyLanceTroops, notable.StringId, notable.BornSettlement.StringId));
            notableTroops.Clear();
        }
        public bool HasLances(PartyBase party)
        {
            return _activeLancesForParties.TryGetValue(party.Id, out var _);
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
        public NotableLanceData GetNotableData(string notableId)
        {
            return _notablesLance[notableId];
        }
        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}