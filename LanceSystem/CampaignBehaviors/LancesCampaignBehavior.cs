using LanceSystem.Deserialization;
using LanceSystem.Dialogues;
using LanceSystem.LanceDataClasses;
using LanceSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace LanceSystem.CampaignBehaviors
{
    public class LancesCampaignBehavior : CampaignBehaviorBase
    {
        [SaveableField(1)]
        Dictionary<string, List<LanceData>> _activeLancesForParties = new();
        [SaveableField(2)]
        Dictionary<string, SettlementNotableLanceInfo> _notablesLance = new();
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, UpdateNotablesLance);
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, me =>
            {
                UpdateLanceTroops(PartyBase.MainParty);
            });
            CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, UpdateMaxNotableTroops);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
            CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, OnHeroPrisonerTaken);
            CampaignEvents.HeroCreated.AddNonSerializedListener(this, OnHeroCreated);
            CampaignEvents.OnTroopsDesertedEvent.AddNonSerializedListener(this, TroopsDeserted);
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, DisbandReturningLanceTroops);
        }

        private void DisbandReturningLanceTroops(MobileParty party, Settlement settlement, Hero hero)
        {
            if (party == null || party.PartyComponent is not DisbandedLancePartyComponent pc)
                return;
            var lanceData = _notablesLance[pc.NotableLanceBelongsTo];
            var maxAmount = lanceData.CachedMaxLanceTroops;
            LanceUtils.TransferTroopsBetweenTroopRosters(party.MemberRoster, lanceData.CurrentNotableLanceTroopRoster, party.MemberRoster.TotalManCount, maxAmount.RoundedResultNumber);
            DestroyPartyAction.Apply(null, party);
        }
        private void TroopsDeserted(MobileParty party, TroopRoster roster)
        {
            if (!HasLances(party.Party)) return;
            UpdateLanceTroops(party.Party); // in case some troops were removed from the lance
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
                DestroyPlayerLances();
        }
        private void DestroyPlayerLances()
        {
            var lances = _activeLancesForParties["player_party"];
            for (int i = lances.Count - 1; i >= 0; i--)
                RemoveLance(lances, lances[i]);
        }
        private void DestroyAllPartyLances(PartyBase party)
        {
            if (!HasLances(party)) return;
            var list = GetOrCreateLances(party);
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
            if (lance is NotableLanceData notableLance)
            {
                _notablesLance[notableLance.NotableId].IsTaken = false;
                _notablesLance[notableLance.NotableId].PartyLanceBelongsTo = null;
            }
            allLances.Remove(lance);
            LanceEvents.OnLanceDisbanded(lance);
        }
        
        private void RemoveLancesIfEmpty(PartyBase party)
        {
            var lances = GetOrCreateLances(party);
            for (int i = lances.Count - 1; i >= 0; i--)
            {
                var lance = lances[i];
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
                    var all = Hero.AllAliveHeroes;
                    var lanceModel = Campaign.Current.Models.LanceModel();
                    lanceModel.UpdateNotablesLanceTroops(notable, _notablesLance[notable.StringId]);
                    _notablesLance[notable.StringId].CachedMaxLanceTroops = Campaign.Current.Models.LanceModel().GetMaxTroopsInLance(notable);
                }
            }
            catch(Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Error in LancesCampaignBehavior.UpdateNotablesLance: {ex.Message}", new TaleWorlds.Library.Color(1,0,0)));
            }
        }
        private bool PlayerHasLanceFromNotable(CharacterObject notable)
        {
            return notable.IsHero && PartyBase.MainParty.Lances().Any(l => l is NotableLanceData nl && nl.NotableId == notable.StringId);
        }
        private void AddDialogs(CampaignGameStarter starter)
        {
            DisbandedLanceDialogs.AddDisbandedLanceDialogs(starter);
            starter.AddPlayerLine("go_to_lance_options", "hero_main_options", "go_to_lance_options", "{=lance_notable_about_lance}About my lance...", 
                () => 
                { return PlayerHasLanceFromNotable(CharacterObject.OneToOneConversationCharacter); },
                null);
            starter.AddDialogLine("lance_options", "go_to_lance_options", "lance_options", "{=lance_yes}Yes?", null, null);
            starter.AddPlayerLine("change_lance_template", "lance_options", "lord_pretalk", "{=lance_template_change}I wish to change the troop template for the lance", null, () =>
            {
                var name = (NotableLanceData)PartyBase.MainParty.Lances().First(l => l is NotableLanceData ld && ld.NotableId == CharacterObject.OneToOneConversationCharacter.StringId);
                var notableLanceData = name.GetSettlementNotableLanceInfo();
                List<InquiryElement> list = new();
                foreach (var template in notableLanceData.GetPossibleTemplates())
                {
                    list.Add(new(template, template.StringId, new BannerImageIdentifier(Clan.PlayerClan.Banner)));
                }
                MultiSelectionInquiryData inquiry = new(new TextObject("{=lance_template_menu}Lance Menu").ToString(), new TextObject("{=lance_choose_template}Choose your lance template").ToString(), list, true, 1, 1, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), 
                list =>
                {
                    var lance = (Lance)list[0].Identifier;
                    notableLanceData.SetLanceTemplate(lance);
                }, list => { });
                MBInformationManager.ShowMultiSelectionInquiry(inquiry, true);
            });
            starter.AddPlayerLine("change_lance_name", "lance_options", "lord_pretalk", "{=lance_name_change}I wish to change the name of the lance", null, () =>
            {
                var lanceData = PartyBase.MainParty.Lances().First(l => l is NotableLanceData nl &&  nl.NotableId == CharacterObject.OneToOneConversationCharacter.StringId);
                var text = new TextInquiryData(new TextObject("{=lance_name_menu}Lance Name").ToString(), new TextObject("{=lance_name_menu_previous_name}Previous Name: ").ToString() + lanceData.Name, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), s =>
                {
                    lanceData.Name = s; 
                }, null);
                InformationManager.ShowTextInquiry(text, true);
            });
            starter.AddPlayerLine("lance_add_new_recruits", "lance_options", "lance_add_new_recruits_response", "{=lance_refill_notable_lance_start}I wish to fill my lance with new recruits",
            () =>
            {
                var notable = CharacterObject.OneToOneConversationCharacter;
                var lance = (NotableLanceData)PartyBase.MainParty.Lances().First(l => l is NotableLanceData nl && nl.NotableId == notable.StringId);
                return lance.GetSettlementNotableLanceInfo().CurrentNotableLanceTroopRoster.TotalManCount > 0 && lance.LanceRoster.TotalManCount < lance.TotalManCount;
            }, null);
            starter.AddDialogLine("lance_add_new_recruits_response", "lance_add_new_recruits_response", "lance_add_new_recruits_playerchoice", "{INFO}",
            () =>
            {
                var notable = CharacterObject.OneToOneConversationCharacter;
                var lance = (NotableLanceData)PartyBase.MainParty.Lances().First(l => l is NotableLanceData nl && nl.NotableId == notable.StringId);
                bool canAsk = lance.LanceRoster.TotalManCount < lance.TotalManCount;
                if (!canAsk) return false;
                GenerateNotableTroopsText();
                return true;
            }, null);
            starter.AddPlayerLine("lance_add_new_recruits_yes", "lance_add_new_recruits_playerchoice", "lord_pretalk", "{=lance_recruitment_take}I will take them", null,
                () =>
                {
                    var notable = CharacterObject.OneToOneConversationCharacter;
                    var lance = (NotableLanceData)PartyBase.MainParty.Lances().First(l => l is NotableLanceData nl && nl.NotableId == notable.StringId);
                    var amountToGet = lance.TotalManCount - lance.LanceRoster.TotalManCount;
                    TroopRoster tempRoster = TroopRoster.CreateDummyTroopRoster();
                    var notableTroopRoster = lance.GetSettlementNotableLanceInfo().CurrentNotableLanceTroopRoster;
                    LanceUtils.TransferTroopsBetweenTroopRosters(notableTroopRoster, tempRoster, amountToGet, lance.TotalManCount);
                    lance.LanceRoster.Add(tempRoster);
                    PartyBase.MainParty.MemberRoster.Add(tempRoster);
                });
            starter.AddPlayerLine("lance_add_new_recruits_no", "lance_add_new_recruits_playerchoice", "lord_pretalk", "{=lance_notable_options_no}Nevermind", null, null, 100, null);

            starter.AddPlayerLine("lance_options_cancel", "lance_options", "lord_pretalk", "{=lance_notable_options_no}Nevermind", null, null, 100, null);
            starter.AddPlayerLine("player_request_lance", "hero_main_options", "lance_1", "{=lance_notable_recruit}I wish to recruit a lance",
                () => { return CharacterObject.OneToOneConversationCharacter.IsHero &&
                    CharacterObject.OneToOneConversationCharacter.HeroObject.IsNotable
                    && CharacterObject.OneToOneConversationCharacter.HeroObject.Occupation != Occupation.GangLeader
                    && !PlayerHasLanceFromNotable(CharacterObject.OneToOneConversationCharacter); }, 
                null, 100, null, null);
            starter.AddDialogLine("lance_no", "lance_1", "hero_main_options", "{REFUSAL_TEXT}", new ConversationSentence.OnConditionDelegate(NotableLanceDialogs.ChooseTextVariationWhenNotableRefusesToEnlistLance), null);
            starter.AddDialogLine("lance_1", "lance_1", "lance_main_options", "{INFO}", new ConversationSentence.OnConditionDelegate(GenerateNotableTroopsText), null, 100, null);
            starter.AddPlayerLine("lance_take", "lance_main_options", "lord_pretalk", "{TAKE_TEXT}", new ConversationSentence.OnConditionDelegate(NotableLanceDialogs.ChooseTextVariationWhenEnlistingLance), () => { GiveTroopsToParty(PartyBase.MainParty, CharacterObject.OneToOneConversationCharacter.HeroObject); }, 100, null);
            starter.AddPlayerLine("lance_no", "lance_main_options", "lord_pretalk", "{REFUSAL_TEXT}", new ConversationSentence.OnConditionDelegate(NotableLanceDialogs.ChooseTextVariantWhenNotTakingLance), null, 100, null);
        }
        private bool GenerateNotableTroopsText()
        {
            var notable = CharacterObject.OneToOneConversationCharacter.HeroObject;
            var troops = _notablesLance[notable.StringId].CurrentNotableLanceTroopRoster;
            string text = "{=lance_notable_recruit_troop_offer}I can offer you the following troops for your lance:\n";
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
            partyLanceTroops.Add(notableTroops);
            var lanceName = GetLanceName(notable, notable.BornSettlement, lanceData.CurrentLance).ToString();
            notableTroops.Clear();
            var lance = new NotableLanceData(partyLanceTroops, notable.StringId, notable.BornSettlement.StringId, lanceName);
            AddLanceToParty(party, lance);
        }
        public void AddLanceToParty(PartyBase party, LanceData lance)
        {
            var lancesList = GetOrCreateLances(party); 
            party.MemberRoster.Add(lance.LanceRoster);
            lancesList.Add(lance);
        }
        public static TextObject GetLanceName(Hero notable, Settlement settlement, Lance lance)
        {
            var text = GameTexts.FindText("str_lance_name", notable.Culture.StringId);
            if (text.Value.Contains("ERROR"))
                text = GameTexts.FindText("str_lance_name", "base");
            GameTexts.SetVariable("TEMPLATE_NAME", lance.Name);
            GameTexts.SetVariable("SETTLEMENT_NAME", settlement.Name);
            GameTexts.SetVariable("NOTABLE_NAME", notable.Name);
            return text;
        }
        public void RemoveLanceFromParty(PartyBase party, LanceData lance)
        {
            if (!HasLances(party))
                return;
            var lances = GetOrCreateLances(party);
            foreach (var troop in lance.LanceRoster.GetTroopRoster())
                party.MemberRoster.RemoveTroop(troop.Character, troop.Number);
            RemoveLance(lances, lance);
        }
        public void DisbandLanceInParty(PartyBase party, int lanceNumber, bool removeTroops) // if disbanded through lance ui, the troops are already removed from the party
        {
            if (party.Lances() == null || party.Lances().Count <= lanceNumber)
            {
                InformationManager.DisplayMessage(new($"Lance system error, {party.Name} does not have {lanceNumber} lances", new Color(1, 0 ,0)));
                return;
            }
            var lanceToDisband = party.Lances()[lanceNumber];
            if (removeTroops)
                foreach (var troop in lanceToDisband.LanceRoster.GetTroopRoster())
                    party.MemberRoster.RemoveTroop(troop.Character, troop.Number);
            RemoveLance(party.Lances(), lanceToDisband);
            if (lanceToDisband is NotableLanceData nl)
                DisbandedLancePartyComponent.CreateDisbandedLanceParty(nl, party);
        }
        public void ReAddLanceToPlayerParty(MobileParty lanceParty)
        {
            if (lanceParty.PartyComponent is not DisbandedLancePartyComponent pc)
                return;
            var lanceData = _notablesLance[pc.NotableLanceBelongsTo];
            lanceData.PartyLanceBelongsTo = MobileParty.MainParty.StringId;
            MobileParty.MainParty.MemberRoster.Add(lanceParty.MemberRoster);

            var notable = MBObjectManager.Instance.GetObject<CharacterObject>(pc.NotableLanceBelongsTo).HeroObject;
            var lanceName = GetLanceName(notable, notable.BornSettlement, lanceData.CurrentLance).ToString();
            var newLance = new NotableLanceData(lanceParty.MemberRoster, pc.NotableLanceBelongsTo, pc.HomeSettlement.StringId, lanceName);
            lanceParty.Party.Lances().Add(newLance);
            DestroyPartyAction.Apply(null, lanceParty);
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
        public SettlementNotableLanceInfo GetNotableData(string notableId)
        {
            return _notablesLance[notableId];
        }
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("activeLancesForParties", ref _activeLancesForParties);
            dataStore.SyncData("notablesLance", ref _notablesLance);
        }
    }
}