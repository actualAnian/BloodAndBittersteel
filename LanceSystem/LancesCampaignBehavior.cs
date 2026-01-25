using LanceSystem.Deserialization;
using LanceSystem.Dialogues;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
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

namespace LanceSystem
{
    public class LancesCampaignBehavior : CampaignBehaviorBase
    {
        [SaveableField(1)]
        Dictionary<string, List<LanceData>> _activeLancesForParties = new();
        [SaveableField(2)]
        Dictionary<string, NotableLanceData> _notablesLance = new();
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

        public NotableLanceData GetNotableLanceData(LanceData data)
        {
            return _notablesLance[data.NotableId];
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
        private bool PlayerHasLanceFromNotable(CharacterObject notable)
        {

            return notable.IsHero && PartyBase.MainParty.Lances().Any(l => l.NotableId == notable.StringId);
        }
        private void AddDialogs(CampaignGameStarter starter)
        {
            DisbandedLanceDialogs.AddDisbandedLanceDialogs(starter);
            starter.AddPlayerLine("go_to_lance_options", "hero_main_options", "go_to_lance_options", "{=lance_about_lance}About my lance...", 
                () => 
                { return PlayerHasLanceFromNotable(CharacterObject.OneToOneConversationCharacter); },
                null);
            starter.AddDialogLine("lance_options", "go_to_lance_options", "lance_options", "{=lance_yes}Yes?", null, null);
            starter.AddPlayerLine("change_lance_template", "lance_options", "lord_pretalk", "{=lance_template_change}I wish to change the troop template for the lance", null, () =>
            {
                var name = PartyBase.MainParty.Lances().First(l => l.NotableId == CharacterObject.OneToOneConversationCharacter.StringId);
                var notableLanceData = name.GetNotableLanceData();
                List<InquiryElement> list = new()
                {
                    //new("aaa", "Stag Riders", new EmptyImageIdentifier()),
                    //new("aaa", "Stag Riders", new BannerImageIdentifier(Clan.PlayerClan.Banner)),
                    //new("aaa", "Stag Riders", new CharacterImageIdentifier(CharacterCode.CreateFrom(Kingdom.All[0].BasicTroop))),
                    //new("aaa", "Stag Riders", new ItemImageIdentifier(Hero.MainHero.BattleEquipment[0].Item))
                };
                foreach (var template in notableLanceData.GetPossibleTemplates())
                {
                    list.Add(new(template, template.StringId, new BannerImageIdentifier(Clan.PlayerClan.Banner)));
                }
                MultiSelectionInquiryData inquiry = new("Lance Menu", "Choose your lance template", list, true, 1, 1, "Confirm", "Cancel", 
                    list =>
                    {
                        var lance = (Lance)list[0].Identifier;
                        notableLanceData.SetLanceTemplate(lance);
                    }, (List<InquiryElement> list) => { });
                MBInformationManager.ShowMultiSelectionInquiry(inquiry, true);
            });
            starter.AddPlayerLine("change_lance_name", "lance_options", "lord_pretalk", "{=lance_name_change}I wish to change the name of the lance", null, () =>
            {
                var lanceData = PartyBase.MainParty.Lances().First(l => l.NotableId == CharacterObject.OneToOneConversationCharacter.StringId);
                var text = new TextInquiryData("Lance Name", "Previous Name: " + lanceData.Name, true, true, "Confirm", "Cancel", s => { }, null);
                InformationManager.ShowTextInquiry(text, true);
            });
            starter.AddPlayerLine("lance_options_cancel", "lance_options", "lord_pretalk", "Nevermind", null, null, 100, null);
            starter.AddPlayerLine("player_request_lance", "hero_main_options", "lance_1", "I wish to recruit a lance",
                () => { return CharacterObject.OneToOneConversationCharacter.IsHero &&
                    CharacterObject.OneToOneConversationCharacter.HeroObject.IsNotable
                    && CharacterObject.OneToOneConversationCharacter.HeroObject.Occupation != Occupation.GangLeader
                    && !PlayerHasLanceFromNotable(CharacterObject.OneToOneConversationCharacter); }, 
                null, 100, null, null);
            starter.AddDialogLine("lance_no", "lance_1", "hero_main_options", "{REFUSAL_TEXT}", new ConversationSentence.OnConditionDelegate(LanceTextVariation.ChooseTextVariationWhenNotableRefusesToEnlistLance), null);
            starter.AddDialogLine("lance_1", "lance_1", "lance_main_options", "{INFO}", new ConversationSentence.OnConditionDelegate(GenerateNotableTroopsText), null, 100, null);
            starter.AddPlayerLine("lance_take", "lance_main_options", "lord_pretalk", "{TAKE_TEXT}", new ConversationSentence.OnConditionDelegate(LanceTextVariation.ChooseTextVariationWhenEnlistingLance), () => { GiveTroopsToParty(PartyBase.MainParty, CharacterObject.OneToOneConversationCharacter.HeroObject); }, 100, null);
            starter.AddPlayerLine("lance_no", "lance_main_options", "lord_pretalk", "{REFUSAL_TEXT}", new ConversationSentence.OnConditionDelegate(LanceTextVariation.ChooseTextVariantWhenNotTakingLance), null, 100, null);
        }
        private static bool GameMenuCanRequestLanceOnCondition(MenuCallbackArgs args)
        {
            bool heroHasLance = CharacterObject.OneToOneConversationCharacter.IsHero
                && CharacterObject.OneToOneConversationCharacter.HeroObject.IsNotable
                && CharacterObject.OneToOneConversationCharacter.HeroObject.Occupation != Occupation.GangLeader;
            if (!heroHasLance) return false;
                
            return true;
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
            var lanceName = GetLanceName(notable, notable.BornSettlement, lanceData.CurrentLance).ToString();
            lancesList.Add(new LanceData(partyLanceTroops, notable.StringId, notable.BornSettlement.StringId, lanceName));
            notableTroops.Clear();
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
            DisbandedLancePartyComponent.CreateDisbandedLanceParty(lanceToDisband, party);
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
            LanceData newLance = new(lanceParty.MemberRoster, pc.NotableLanceBelongsTo, pc.HomeSettlement.StringId, lanceName);
            lanceParty.Party.Lances().Add(newLance);

            DestroyPartyAction.Apply(null, lanceParty);
            //DisbandPartyAction.StartDisband(lanceParty);    
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
            dataStore.SyncData("activeLancesForParties", ref _activeLancesForParties);
            dataStore.SyncData("notablesLance", ref _notablesLance);
        }
    }
}