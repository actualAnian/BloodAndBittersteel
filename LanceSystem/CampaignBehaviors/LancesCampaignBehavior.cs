using HarmonyLib;
using LanceSystem.Deserialization;
using LanceSystem.Dialogues;
using LanceSystem.LanceDataClasses;
using LanceSystem.Logger;
using LanceSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using static TaleWorlds.CampaignSystem.MapEvents.MapEvent;

namespace LanceSystem.CampaignBehaviors
{
    [Serializable]
    public class NotableDeathRecord
    {
        public string DeadNotableId { get; set; }
        public Settlement HomeSettlement { get; set; }
        public Occupation Occupation { get; set; }
        public bool WasLanceTaken { get; set; }
        public string? PartyThatHasLance { get; set; }

        public NotableDeathRecord(string id, Settlement settlement, Occupation occ, bool wasTaken, string? partyThatHasLance = null)
        {
            DeadNotableId = id;
            HomeSettlement = settlement;
            Occupation = occ;
            WasLanceTaken = wasTaken;
            PartyThatHasLance = partyThatHasLance;
        }
    }
    public class LancesCampaignBehavior : CampaignBehaviorBase
    {
        public static LancesCampaignBehavior Instance => Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
        static readonly Random _random = new();
        [SaveableField(1)]
        Dictionary<string, List<LanceData>> _activeLancesForParties = new();
        [SaveableField(2)]
        Dictionary<string, SettlementNotableLanceInfo> _notablesLance = new();
        [SaveableField(3)]
        CampaignTime _mercRecruitTimeSpan = CampaignTime.Zero;
        [SaveableField(4)]
        Dictionary<string, NotableDeathRecord> _pendingNotableDeaths = new();
        List<string> LockedParties = new();

        public bool CanRecruitDisbandedLanceAsMercenaries()
        {
            if (_mercRecruitTimeSpan > CampaignTime.Now) return false;
            float chance = Clan.PlayerClan.Tier switch
            {
                0 or 1 => 0.2f,
                2 => 0.5f,
                _ => 0.8f,
            };
            var canRecruit = _random.NextDouble() < chance;
            _mercRecruitTimeSpan = CampaignTime.HoursFromNow(6);
            return canRecruit;
        }
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, UpdateNotablesLance);
            CampaignEvents.OnPartySizeChangedEvent.AddNonSerializedListener(this, OnPartySizeChanged);
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnNotableHeroKilled);
            CampaignEvents.HeroCreated.AddNonSerializedListener(this, OnNotableHeroCreated);
            CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, UpdateMaxNotableTroops);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
            CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, OnHeroPrisonerTaken);
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, DisbandReturningLanceTroops);
            CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, FillNotablesData);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, RemoveLancesIfNotableDiedWithNoHeir);
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, RemoveLancesFromLostSettlement);
            LanceEvents.AiUpgradeTroops.AddNonSerializedListener(this, OnAiUpgradeTroops);
        }

        private void RemoveLancesFromLostSettlement(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var notables = new List<Hero>(settlement.Notables);

            foreach (var village in settlement.BoundVillages)
                notables.AddRange(village.Settlement.Notables);
            foreach (var notable in notables)
            {
                if (_notablesLance.TryGetValue(notable.StringId, out var info))
                {
                    if (!info.IsTaken)
                        continue;
                    var lordParty = MobileParty.AllLordParties.FirstOrDefault(p => p.StringId == info.PartyLanceBelongsTo);
                    var lances = GetOrCreateLances(lordParty.Party);
                    var lanceToRemoveIndex = lances.FindIndex(l => l is NotableLanceData notableLance && notableLance.NotableId == info.NotableId);
                    if (lordParty == MobileParty.MainParty)
                    {
                        var lanceDisbandedText = $"{{=lance_lost_settlement_text}}The lance {lances[lanceToRemoveIndex].Name} has been disbanded as the notable no longer answers to you.";
                        var inquiry = new InquiryData(new TextObject("{=lance_lost_title}Lance Lost!").ToString(), new TextObject(lanceDisbandedText).ToString(), true, false, new TextObject("{=WVkc4UgX}Continue.").ToString(), "", () => { }, () => { });
                        InformationManager.ShowInquiry(inquiry, true, false);
                    }
                    DisbandLanceInParty(lordParty.Party, lanceToRemoveIndex, false);
                }
            }
        }
        private void RemoveLancesIfNotableDiedWithNoHeir()
        {
            foreach(var key in _notablesLance.Keys.ToList())
            {
                var data = _notablesLance[key];
                if (data.IsValid) continue;

                if (data.IsTaken)
                {
                    var lordParty = MobileParty.AllLordParties.FirstOrDefault(p => p.StringId == data.PartyLanceBelongsTo);
                    var lordLances = GetOrCreateLances(lordParty.Party);
                    var lanceToDisband = lordLances.FirstOrDefault(l => l is NotableLanceData nl && nl.GetSettlementNotableLanceInfo() == data);
                    if (lanceToDisband != null)
                    {
                        if (lordParty == MobileParty.MainParty)
                        {
                            var lanceDisbandedText = $"{{=lance_notable_lost_text}}The lance {lanceToDisband.Name} has been disbanded because the notable has died with no heir.";
                            var inquiry = new InquiryData(new TextObject("{=lance_lost_title}Lance Lost!").ToString(), new TextObject(lanceDisbandedText).ToString(), true, false, new TextObject("{=WVkc4UgX}Continue.").ToString(), "", () => { }, () => { });
                            InformationManager.ShowInquiry(inquiry, true, false);
                        }
                        DisbandLanceInParty(lordParty.Party, lanceToDisband, false);
                    }
                }
                _pendingNotableDeaths.Remove(key);
                _notablesLance.Remove(key);
            }
        }

        private void OnNotableHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
        {
            if (!victim.IsNotable || !_notablesLance.ContainsKey(victim.StringId))
                return;

            var oldData = _notablesLance[victim.StringId];
            oldData.IsValid = false;
            if (oldData.IsTaken)
            _pendingNotableDeaths[victim.StringId] = new NotableDeathRecord(
                victim.StringId,
                victim.HomeSettlement,
                victim.Occupation,
                oldData.IsTaken,
                oldData.PartyLanceBelongsTo);
        }

        private void OnPartySizeChanged(PartyBase party)
        {
            //if (party == PartyBase.MainParty) return;
            UpdateLanceTroops(party);
        }

        private void OnAiUpgradeTroops(PartyBase party, CharacterObject from, CharacterObject to, int amount)
        {
            if (!Campaign.Current.Models.LanceModel().IsUsingLanceSystem(party)) return;
            LanceUtils.UpgradeTroopsRandomlyInLances(from, to, amount, party.Lances());
        }

        private void FillNotablesData(CampaignGameStarter starter, int arg2)
        {
            var lanceModel = Campaign.Current.Models.LanceModel();
            foreach (var data in _notablesLance)
            {
                var lanceData = data.Value;
                var character = MBObjectManager.Instance.GetObject<CharacterObject>(data.Key);
                var notable = character.HeroObject;
                //lanceData.SetRandomLanceTemplateWeighted();
                while (lanceData.CachedMaxLanceTroops.RoundedResultNumber > lanceData.CurrentNotableLanceTroopRoster.TotalHealthyCount)
                    lanceModel.UpdateNotablesLanceTroops(notable, lanceData);
            }
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
        private void OnNotableHeroCreated(Hero hero, bool isBornNaturally)
        {
            var notableOccupations = new[]
            {
                Occupation.GangLeader,
                Occupation.Artisan,
                Occupation.Merchant,
                Occupation.RuralNotable,
                Occupation.Headman
            };

            if (!notableOccupations.Contains(hero.Occupation))
                return;

            foreach (var kvp in _pendingNotableDeaths.ToList())
            {
                var record = kvp.Value;

                if (record.HomeSettlement != hero.HomeSettlement || record.Occupation != hero.Occupation)
                    continue;
                var oldInfo = _notablesLance[kvp.Key];
                var newData = new SettlementNotableLanceInfo(hero, oldInfo.CurrentNotableLanceTroopRoster, oldInfo.IsTaken);
                newData.SetLanceTemplate(oldInfo.CurrentLance);

                _notablesLance[hero.StringId] = newData;
                if (record.WasLanceTaken && !string.IsNullOrEmpty(record.PartyThatHasLance))
                {
                    var newTargetPartyLance = record.PartyThatHasLance;

                    MobileParty lordParty = MobileParty.AllLordParties.FirstOrDefault(p => p.StringId == record.PartyThatHasLance);
                    if (lordParty == null) continue;
                    foreach (NotableLanceData lanceData in GetOrCreateLances(lordParty.Party).Cast<NotableLanceData>())
                    {
                        lanceData.NotableId = hero.StringId;
                        newData.PartyLanceBelongsTo = newTargetPartyLance;
                    }
                }
                else if (!record.WasLanceTaken)
                {
                    newData.IsTaken = false;
                    newData.PartyLanceBelongsTo = null;
                }
                _notablesLance.Remove(kvp.Key);
                _pendingNotableDeaths.Remove(kvp.Key);
                break; // Only one notable can replace one other
            }

            if (!_notablesLance.ContainsKey(hero.StringId))
            {
                var dummyRoster = TroopRoster.CreateDummyTroopRoster();
                _notablesLance[hero.StringId] = new SettlementNotableLanceInfo(hero, dummyRoster, false);
            }
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
        private void CheckLanceConsistency(PartyBase party)
        {
            if (party.Owner.Culture.StringId == "battania") return;
            var tempRoster = TroopRoster.CreateDummyTroopRoster();
            foreach(var lance in party.Lances())
            {
                tempRoster.Add(lance.LanceRoster);
            }
            foreach(var troop in tempRoster.GetTroopRoster())
            {
                if (party.MemberRoster.GetTroopCount(troop.Character) < tempRoster.GetTroopCount(troop.Character))
                {
                    LanceUtils.NormalizeLanceTroopsToParty(party.MemberRoster, party.Lances());
                    LanceLogger.Logger.Warning($"Lance inconsistency for party {party.LeaderHero?.StringId}. and troop {troop.Character.Name}");
                }
            }
        }
        public void UpdateLanceTroops(PartyBase party)
        {
            var lances = GetOrCreateLances(party);
            if (lances.Count == 0)
                return;
            if (LockedParties.Contains(party.Id)) return;
            LanceUtils.NormalizeLanceTroopsToParty(party.MemberRoster, party.Lances());
            RemoveLancesIfEmpty(party);
            CheckLanceConsistency(party);
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
                return lance.GetSettlementNotableLanceInfo().CurrentNotableLanceTroopRoster.TotalManCount > 0 && lance.LanceRoster.TotalManCount < lance.MaxSize;
            }, null);
            starter.AddDialogLine("lance_add_new_recruits_response", "lance_add_new_recruits_response", "lance_add_new_recruits_playerchoice", "{INFO}",
            () =>
            {
                var notable = CharacterObject.OneToOneConversationCharacter;
                var lance = (NotableLanceData)PartyBase.MainParty.Lances().First(l => l is NotableLanceData nl && nl.NotableId == notable.StringId);
                bool canAsk = lance.LanceRoster.TotalManCount < lance.MaxSize;
                if (!canAsk) return false;
                GenerateNotableTroopsText();
                return true;
            }, null);
            starter.AddPlayerLine("lance_add_new_recruits_yes", "lance_add_new_recruits_playerchoice", "lord_pretalk", "{=lance_recruitment_take}I will take them", null,
                () =>
                {
                    var notable = CharacterObject.OneToOneConversationCharacter;
                    var lance = (NotableLanceData)PartyBase.MainParty.Lances().First(l => l is NotableLanceData nl && nl.NotableId == notable.StringId);
                    RefillLanceTroops(lance, PartyBase.MainParty);
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
            starter.AddPlayerLine("lance_take", "lance_main_options", "lord_pretalk", "{TAKE_TEXT}", new ConversationSentence.OnConditionDelegate(NotableLanceDialogs.ChooseTextVariationWhenEnlistingLance), () => { RecruitNotableLanceToParty(PartyBase.MainParty, CharacterObject.OneToOneConversationCharacter.HeroObject); }, 100, null);
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
        public void RefillLanceTroops(NotableLanceData lance, PartyBase party)
        {
            var amountToGet = lance.MaxSize - lance.LanceRoster.TotalManCount;
            if (amountToGet == 0) return;
            TroopRoster tempRoster = TroopRoster.CreateDummyTroopRoster();
            var notableTroopRoster = lance.GetSettlementNotableLanceInfo().CurrentNotableLanceTroopRoster;
            LanceUtils.TransferTroopsBetweenTroopRosters(notableTroopRoster, tempRoster, amountToGet, lance.MaxSize);
            lance.LanceRoster.Add(tempRoster);
            party.MemberRoster.Add(tempRoster);
        }
        public void RecruitNotableLanceToParty(PartyBase party, Hero notable)
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
        public static string GetLanceName(Hero notable, Settlement settlement, Lance lance)
        {
            var text = GameTexts.FindText("str_lance_name", notable.Culture.StringId);
            if (text.Value.Contains("ERROR"))
                text = GameTexts.FindText("str_lance_name", "base");
            GameTexts.SetVariable("TEMPLATE_NAME", lance.Name);
            GameTexts.SetVariable("SETTLEMENT_NAME", settlement.Name);
            GameTexts.SetVariable("NOTABLE_NAME", notable.Name);
            return text.ToString();
        }
        public void RemoveLanceFromParty(PartyBase party, LanceData lance)
        {
            if (!HasLances(party))
                return;
            var lances = GetOrCreateLances(party);
            RemoveTroopsFromLancesSafely(party, lance.LanceRoster);
            RemoveLance(lances, lance);
        }
        public static FieldInfo troopRosterData = AccessTools.Field("TaleWorlds.CampaignSystem.Roster.TroopRoster:data");
        public void RemoveTroopsFromLancesSafely(PartyBase party, TroopRoster troopsToRemove)
        {
            LockedParties.Add(party.Id);
            foreach (var troop in (TroopRosterElement[])troopRosterData.GetValue(troopsToRemove))
            {
                if (troop.Character == null) break;
                party.MemberRoster.RemoveTroop(troop.Character, troop.Number);
            }
            LockedParties.Remove(party.Id);
        }
        /*
         * WARNING, potentially unsafe. needed for LanceParty VM
         * only use in extreme cases, otherwise use RemoveTroopsFromLancesSafely
         */
        public void LockParty(PartyBase party)
        {
            if (!LockedParties.Contains(party.Id))
                LockedParties.Add(party.Id);
        }
        public void UnlockParty(PartyBase party)
        {
            if (LockedParties.Contains(party.Id))
                LockedParties.Remove(party.Id);
        }
        public void RemoveTroopsFromLancesSafely(TroopRoster removeFrom, TroopRoster troopsToRemove)
        {
            var ownerParty = AccessTools.Property("TaleWorlds.CampaignSystem.Roster.TroopRoster:OwnerParty").GetGetMethod(true);
            var party = (PartyBase)ownerParty.Invoke(removeFrom, null);
            LockedParties.Add(party.Id);
            foreach (var troop in (TroopRosterElement[])troopRosterData.GetValue(troopsToRemove))
            {
                if (troop.Character == null) break;
                removeFrom.RemoveTroop(troop.Character, troop.Number);
            }
            LockedParties.Remove(party.Id);
        }
        public void DisbandLanceInParty(PartyBase party, LanceData lanceToDisband, bool removeTroops)
        {
            CheckLanceConsistency(party);
            try
            {
                if (removeTroops)
                    RemoveTroopsFromLancesSafely(party, lanceToDisband.LanceRoster);
                RemoveLance(party.Lances(), lanceToDisband);
                if (lanceToDisband is NotableLanceData nl && removeTroops)
                    DisbandedLancePartyComponent.CreateDisbandedLanceParty(nl, party);
            }
            catch(Exception)
            {
                LanceLogger.Logger.Warning($"Error disbanding lance in party {party.Name} of lord {party.LeaderHero?.Name}");
            }
        }
        public void DisbandLanceInParty(PartyBase party, int lanceNumber, bool removeTroops) // if disbanded through lance ui, the troops are already removed from the party
        {
            if (party.Lances() == null || party.Lances().Count <= lanceNumber)
            {
                InformationManager.DisplayMessage(new($"Lance system error, {party.Name} does not have {lanceNumber} lances", new Color(1, 0 ,0)));
                return;
            }
            var lanceToDisband = party.Lances()[lanceNumber];
            DisbandLanceInParty(party, lanceToDisband, removeTroops);
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
        public float GetMostReadyLance(Settlement settlement)
        {
            float filled = 0;
            foreach (var notable in settlement.Notables)
            {
                if (_notablesLance.TryGetValue(notable.StringId, out var lanceData))
                {
                    if (lanceData.IsTaken) continue;
                    float value = (float)lanceData.CurrentNotableLanceTroopRoster.TotalManCount / lanceData.CachedMaxLanceTroops.ResultNumber;
                    if (filled < value) filled = value;
                    if (filled >= 1f) break;
                }
            }
            if (filled < AILanceRecruitment.FilledLancePercentageToConsiderTaking) filled = 0;
            return filled;
        }
        public bool DoesSettlementHaveFreeLances(Settlement settlement)
        {
            foreach (var notable in settlement.Notables)
                if (_notablesLance.TryGetValue(notable.StringId, out var lanceData))
                    if (!lanceData.IsTaken) return true;
            return false;
        }

        public Hero? GetHeroWithStrongestLanceInSettlement(Settlement settlement)
        {
            float strongestLanceStrength = 0;
            Hero? hero = null;
            foreach (var notable in settlement.Notables)
            {
                if (_notablesLance.TryGetValue(notable.StringId, out var lanceData))
                {
                    if (lanceData.IsTaken) continue;
                    PowerCalculationContext context = Campaign.Current.Models.MilitaryPowerModel.GetContextForPosition(settlement.Position);
                    var strength = TroopRosterExtensions.CalculateTroopRosterStrength(lanceData.CurrentNotableLanceTroopRoster, BattleSideEnum.Defender, context);
                    if (strength > strongestLanceStrength)
                    {
                        strongestLanceStrength = strength;
                        hero = notable;
                    }
                }
            }
            return hero;
        }
        public SettlementNotableLanceInfo GetNotableData(string notableId)
        {
            return _notablesLance[notableId];
        }
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("activeLancesForParties", ref _activeLancesForParties);
            dataStore.SyncData("notablesLance", ref _notablesLance);
            dataStore.SyncData("mercRecruitTimeSpan", ref _mercRecruitTimeSpan);
            dataStore.SyncData("pendingNotableDeaths", ref _pendingNotableDeaths);
        }
    }
}