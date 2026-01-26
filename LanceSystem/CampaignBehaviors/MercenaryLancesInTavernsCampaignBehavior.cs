using Helpers;
using LanceSystem.Deserialization;
using LanceSystem.Dialogues;
using LanceSystem.LanceDataClasses;
using LanceSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using static TaleWorlds.CampaignSystem.MapEvents.MapEvent;

namespace LanceSystem.CampaignBehaviors
{
    public partial class MercenaryLancesInTavernsCampaignBehavior : CampaignBehaviorBase
    {
        readonly Random random = new();
        [SaveableField(1)]
        List<MercenaryLanceData> _mercData = new();
        [SaveableField(2)]
        private Dictionary<string, CampaignTime> _lastRequestTimes = new();
        [SaveableField(3)]
        private Dictionary<string, CampaignTime> _townsWithMercenaries = new();
        TemporaryMercData? _tempMercData;
        TemporaryRefillData? _tempRefillData;
        bool _temporaryFinishedBusinessWithMerc = false;
        int _flavorLingeringWithMercenary = 0;
        public static int ContractLengthInDays => CampaignTime.DaysInSeason;
        public static int CooldownBeforeMercenaryIsInTownInDays => 7; // also cooldown how long mercenary stays in town
        public static int GetLanceSizeFromClan(Clan clan)
        {
            return clan.Tier switch
            {
                0 or 1 => 20,
                2 => 30,
                3 => 30,
                4 => 40,
                5 => 50,
                _ => 50,
            };
        }
        public static float PlayerPartyMutinyChance
        {
            get
            {
                return Clan.PlayerClan.Tier switch
                {
                    1 => 0.8f,
                    2 => 0.5f,
                    _ => 0.0f,
                };
            }
        }
        public float ChanceToSpawnMercenary(Settlement settlement)
        {
            return 0.3f;
        }

        public int BuyMercenaryMultiplier => CampaignTime.DaysInSeason;
        public override void RegisterEvents()
        {
            LanceEvents.LanceDisbanded.AddNonSerializedListener(this, OnLanceDisbanded);
            CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(TryAddMercenaryToTavern));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, AddDialogs);
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, ResetTemporaryData);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTick);
        }
        private void OnLanceDisbanded(LanceData data)
        {
            if (data is MercenaryLanceData md)
                _mercData.Remove(md);
        }
        private void EnterRenegotiateContractDialog(MercenaryLanceData mercenaryLance)
        {
            var playerData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty);
            var renegotiationCost = CalculateLanceCost(mercenaryLance.LanceRoster);
            if (mercenaryLance.HasMissedPayment) renegotiationCost *= 2;
            _temporaryRenegotiationData = new(mercenaryLance, renegotiationCost);
            _renegotiationInProgress = true;
            var mercLeader = mercenaryLance.LanceRoster.GetCharacterAtIndex(0);
            var merData = new ConversationCharacterData(mercLeader);
            CampaignMapConversation.OpenConversation(playerData, merData);
        }
        public void RenewContractFinalize(MercenaryLanceData lance, int contractCost)
        {
            lance.ContractEndTime = CampaignTime.DaysFromNow(ContractLengthInDays);
            lance.HasMissedPayment = false;
            GiveGoldAction.ApplyBetweenCharacters(PartyBase.MainParty.Owner, null, contractCost, false);
        }
        private void RenewContract(MercenaryLanceData mercenaryLance)
        {
            try
            {
                var partyBelongedTo = MBObjectManager.Instance.GetObject<CharacterObject>(mercenaryLance.PartyBelongedToStringId);
                if (partyBelongedTo == CharacterObject.PlayerCharacter) EnterRenegotiateContractDialog(mercenaryLance);
                else
                {
                    RenewContractFinalize(mercenaryLance, CalculateLanceCost(mercenaryLance.LanceRoster));
                }
            }
            catch
            {
                InformationManager.DisplayMessage(new($"Error in MercenaryLanceCampaignBehavior.RenewContract for party {mercenaryLance.PartyBelongedToStringId}", new Color(1, 0, 0)));
            }
        }
        private void DailyTick()
        {
            foreach (var contract in _mercData)
            {
                var timeLeft = contract.ContractEndTime - CampaignTime.Now;
                if (timeLeft.ToDays < 0)
                {
                    RenewContract(contract);
                    return;
                }
                if (contract.PartyBelongedToStringId == Hero.MainHero.StringId)
                {
                    var hasUnpaidWages = MobileParty.MainParty.HasUnpaidWages;
                    if (hasUnpaidWages > 0) contract.HasMissedPayment = true;
                    var roundedDaysLeft = (int)Math.Round(timeLeft.ToDays);
                    if (timeLeft.ToDays < 7 || roundedDaysLeft == CampaignTime.DaysInWeek * 2 || roundedDaysLeft == CampaignTime.DaysInWeek * 3)
                    {
                        TextObject message = new("{=lance_merc_time_left}{DAYS_LEFT} days left till the contract with the {TEMPLATE_NAME} expires.");
                        GameTexts.SetVariable("DAYS_LEFT", roundedDaysLeft);
                        GameTexts.SetVariable("TEMPLATE_NAME", contract.Name);
                        InformationManager.DisplayMessage(new(message.ToString(), new Color(1, 0, 1)));
                    }
                }
            }
        }

        private void ResetTemporaryData(IMission mission)
        {
            _tempMercData = null;
            _temporaryFinishedBusinessWithMerc = false;
            _tempRefillData = null;
            _renegotiationInProgress = false;
            _temporaryRenegotiationData = null;
            _flavorLingeringWithMercenary = 0;
        }

        public class TemporaryMercData
        {
            public CharacterObject MercenaryLeader;
            public Lance Template;
            public TroopRoster Troops;
            public int Cost;
            public TemporaryMercData(CharacterObject mercenaryLeader, Lance template, TroopRoster troops, int cost)
            {
                MercenaryLeader = mercenaryLeader;
                Template = template;
                Troops = troops;
                Cost = cost;
            }
        }
        public class TemporaryRefillData
        {
            public MercenaryLanceData LanceData;
            public TroopRoster RefillTroopRoster;
            public int AmountToRefill;
            public int TotalCost;
            public TemporaryRefillData(MercenaryLanceData lanceData, TroopRoster refillTroopRoster, int amountToRefill, int totalCost)
            {
                LanceData = lanceData;
                RefillTroopRoster = refillTroopRoster;
                AmountToRefill = amountToRefill;
                TotalCost = totalCost;
            }
        }
        public class TemporaryRenegotiationData
        {
            public MercenaryLanceData LanceData;
            public int RenegotiationCost;
            public bool isMutiny = false;
            public TemporaryRenegotiationData(MercenaryLanceData lanceData, int renegotiationCost)
            {
                LanceData = lanceData;
                RenegotiationCost = renegotiationCost;
            }
        }
        bool _renegotiationInProgress = false;
        TemporaryRenegotiationData? _temporaryRenegotiationData;
        private void AddMercenaryRenegotiationDialog(CampaignGameStarter starter)
        {
            starter.AddDialogLine(
                "lance_renegotiate_contract_start",
                "start",
                "lance_renegotiate_contract_talk",
                "{RENEGOTIATION_TEXT}",
                () =>
                {
                    if (!_renegotiationInProgress) return false;
                    _renegotiationInProgress = false;
                    var text = new TextObject(MercenaryLanceDialogs.GetHireAgreeDialog(_temporaryRenegotiationData!.LanceData.HasMissedPayment));
                    GameTexts.SetVariable("RENEGOTIATION_TEXT", text);
                    GameTexts.SetVariable("TOTAL_AMOUNT", _temporaryRenegotiationData.RenegotiationCost);
                    return true;
                },
                null
            );
            starter.AddPlayerLine(
                "lance_renegotiate_contract_yes",
                "lance_renegotiate_contract_talk",
                "close_window",
                "{=lance_mercenary_renegotiate_agree}Very well, the contract stands",
                () => { return HasEnoughToPayForRenegotiation(PartyBase.MainParty); },
                () =>
                {
                    RenewContractFinalize(_temporaryRenegotiationData!.LanceData!, _temporaryRenegotiationData.RenegotiationCost);
                }
            );
            starter.AddPlayerLine(
                "lance_renegotiate_contract_no",
                "lance_renegotiate_contract_talk",
                "lance_renegotiate_contract_end",
                "{WILL_NOT_PAY}",
                () =>
                {
                    TextObject text;
                    if (HasEnoughToPayForRenegotiation(PartyBase.MainParty))
                        text = new TextObject("I owe you nothing more. This ends now");
                    else text = new TextObject("I cannot afford it right now. I am ending the contract");
                    GameTexts.SetVariable("WILL_NOT_PAY", text);
                    return true;
                },
                () =>
                {
                    _temporaryRenegotiationData!.isMutiny = WillMercenaryMutiny(_temporaryRenegotiationData.LanceData);
                }
            );
            starter.AddPlayerLine(
                "lance_renegotiate_contract_end_accept",
                "lance_renegotiate_contract_end",
                "close_window",
                "{=lance_mercenary_renegotiate_end}Very well. Perhaps our paths will cross again",
                () =>
                {
                    return !_temporaryRenegotiationData!.isMutiny;
                },
                null
            );
            starter.AddDialogLine(
                "lance_renegotiate_contract_end_threat",
                "lance_renegotiate_contract_end",
                "lance_renegotiate_contract_mutiny",
                "{THREAT_TEXT}",
                () =>
                {
                    var isMutiny = _temporaryRenegotiationData!.isMutiny;
                    if (!isMutiny) return false;
                    var text = MercenaryLanceDialogs.GetThreatExplanationDialog();
                    GameTexts.SetVariable("THREAT_TEXT", text);
                    return true;
                },
                null
            );
            starter.AddDialogLine(
                "lance_renegotiate_contract_mutiny",
                "lance_renegotiate_contract_mutiny",
                "lance_renegotiate_contract_mutiny_response",
                "{THREAT_TEXT}",
                () =>
                {
                    var text = MercenaryLanceDialogs.GetMutinyDialog();
                    GameTexts.SetVariable("THREAT_TEXT", text);

                    return true;
                },
                null
            );
            starter.AddPlayerLine(
                "lance_renegotiate_contract_mutiny_response_pay",
                "lance_renegotiate_contract_mutiny_response",
                "close_window",
                "{=lance_mercenary_mutiny_agree}I will pay",
                () => { return HasEnoughToPayForRenegotiation(PartyBase.MainParty); },
                () =>
                {
                    RenewContractFinalize(_temporaryRenegotiationData!.LanceData!, _temporaryRenegotiationData.RenegotiationCost);
                }
            );
            starter.AddPlayerLine(
                "lance_renegotiate_contract_mutiny_response_not_pay",
                "lance_renegotiate_contract_mutiny_response",
                "close_window",
                "{TO_ARMS}",
                () =>
                {
                    TextObject text;
                    if (HasEnoughToPayForRenegotiation(PartyBase.MainParty))
                        text = new TextObject("{=lancel_player_mutiny_has_money}I owe you nothing. If you want to taste my blade, so you will.");
                    else text = new TextObject("{=lancel_player_mutiny_no_money}You turn on your own commander for coin? You’ll hang for this, every last one of you.");
                    GameTexts.SetVariable("TO_ARMS", text);
                    return true;
                },
                () =>
                {
                    CreateMutiny(_temporaryRenegotiationData!);
                }
            );
        }
        private bool HasEnoughToPayForRenegotiation(PartyBase party)
        {
            return party.Owner.Gold >= _temporaryRenegotiationData!.RenegotiationCost;
        }
        private bool IsTalkingToMercenary()
        {
            return PlayerEncounter.EncounterSettlement != null
                   && PlayerEncounter.EncounterSettlement.IsTown
                   && CampaignMission.Current.Location?.StringId == "tavern"
                   && _tempMercData != null
                   && CharacterObject.OneToOneConversationCharacter == _tempMercData.MercenaryLeader;
        }

        private void CreateMutiny(TemporaryRenegotiationData data)
        {
            var behavior = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
            var mutinousTroops = data.LanceData.LanceRoster.CloneRosterData();
            behavior.RemoveLanceFromParty(PartyBase.MainParty, data.LanceData);
            var settlement = SettlementHelper.FindNearestSettlementToMobileParty(MobileParty.MainParty, MobileParty.NavigationType.All, x => x.IsVillage || x.IsTown);
            PartyTemplateObject looterTemplate = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("looters_template");

            var mutinousParty = BanditPartyComponent.CreateLooterParty("mutinous_mercenaries", Clan.BanditFactions.First(), settlement, false, looterTemplate, MobileParty.MainParty.Position);
            mutinousParty.MemberRoster.Clear();
            mutinousParty.InitializeMobilePartyAtPosition(mutinousTroops, TroopRoster.CreateDummyTroopRoster(), MobileParty.MainParty.Position);
            mutinousParty.SetMoveEngageParty(MobileParty.MainParty, MobileParty.NavigationType.Default);
            PlayerEncounter.Start();
            PlayerEncounter.Current.SetupFields(mutinousParty.Party, PartyBase.MainParty);
            PlayerEncounter.StartBattle();
        }
        private bool WillMercenaryMutiny(MercenaryLanceData lance)
        {
            if (PlayerEncounter.Current != null
                || !lance.HasMissedPayment
                || random.NextFloat() > PlayerPartyMutinyChance
                || !IsMutinyStrongEnough(lance)) return false;
            return true;
        }
        private bool IsMutinyStrongEnough(MercenaryLanceData lance)
        {
            var partyWithoutLance = PartyBase.MainParty.MemberRoster.CloneRosterData();
            foreach (var troop in lance.LanceRoster.GetTroopRoster())
                partyWithoutLance.RemoveTroop(troop.Character, troop.Number);
            PowerCalculationContext context = Campaign.Current.Models.MilitaryPowerModel.GetContextForPosition(MobileParty.MainParty.Position);
            var playerStrength = TroopRosterExtensions.CalculateTroopRosterStrength(partyWithoutLance, BattleSideEnum.Defender, context);
            var mutinyStrength = TroopRosterExtensions.CalculateTroopRosterStrength(lance.LanceRoster, BattleSideEnum.Attacker, context);
            return mutinyStrength * 1.2f > playerStrength;
        }
        private void AddDialogs(CampaignGameStarter starter)
        {
            AddMercenaryRenegotiationDialog(starter);
            starter.AddDialogLine(
                "lance_recruit_mercenaries_start",
                "start",
                "lance_recruit_mercenaries_talk",
                "{MERCENARY_GREETING}",
                () =>
                {
                    var canDialogStart = IsTalkingToMercenary() && !_temporaryFinishedBusinessWithMerc;
                    if (!canDialogStart)
                        return false;
                    var text = MercenaryLanceDialogs.GetGreetingDialogs(PlayerEncounter.EncounterSettlement);
                    MBTextManager.SetTextVariable("MERCENARY_GREETING", new TextObject(text));
                    return true;
                },
                null
            );
            starter.AddDialogLine(
                "lance_recruit_mercenaries_finished_business",
                "start",
                "close_window",
                "{FINISHED_DIALOG}",
                () =>
                {
                    var CanDialogStart = IsTalkingToMercenary() && _temporaryFinishedBusinessWithMerc;
                    if (!CanDialogStart)
                        return false;
                    _flavorLingeringWithMercenary++;
                    var text = MercenaryLanceDialogs.GetFinishedBusinessDialog(_flavorLingeringWithMercenary);
                    MBTextManager.SetTextVariable("FINISHED_DIALOG", new TextObject(text));
                    return true;
                },
                null
            );
            starter.AddPlayerLine(
                "lance_mercenary_hire",
                "lance_recruit_mercenaries_talk",
                "lance_mercenary_hire_response",
                "{=lance_mercenary_hire}I would hire your lance. Name your price.",
                null,
                null
            );
            starter.AddPlayerLine(
                "lance_mercenary_refill",
                "lance_recruit_mercenaries_talk",
                "lance_mercenary_refill_response",
                "{=lance_mercenary_refill}My mercenary lance needs fresh men. Can you refill its ranks?",
                () =>
                {
                    return PartyBase.MainParty.Lances().Any(l => l is MercenaryLanceData && l.LanceRoster.Count < l.TotalManCount);
                },
                null
            );
            starter.AddPlayerLine(
                "lance_mercenary_leave",
                "lance_recruit_mercenaries_talk",
                "close_window",
                "{=lance_mercenary_leave}Another time, then.",
                null,
                null
            );
            starter.AddDialogLine(
                "lance_mercenary_hire_response",
                "lance_mercenary_hire_response",
                "lance_mercenary_hire",
                "{HIRE_AGREE}",
                () =>
                {
                    if (Clan.PlayerClan.Tier == 0) return false;
                    if (!CanHireNewMercenaryLance()) return false;
                    var text = MercenaryLanceDialogs.GetHireAgreeDialog();
                    GameTexts.SetVariable("HIRE_AGREE", text);
                    GameTexts.SetVariable("TOTAL_AMOUNT", _tempMercData!.Cost);
                    return true;
                }, null
            );
            starter.AddPlayerLine(
                "lance_mercenary_hire_yes",
                "lance_mercenary_hire",
                "close_window",
                "{=lance_merc_player_agree}You ride with me now",
                () =>
                {
                    return HasEnoughGold(PartyBase.MainParty);
                },
                () => { _temporaryFinishedBusinessWithMerc = true; HireMercenaryLance(PartyBase.MainParty, PlayerEncounter.EncounterSettlement, _tempMercData!.Troops); }
            );
            starter.AddPlayerLine(
                "lance_mercenary_hire_no",
                "lance_mercenary_hire",
                "close_window",
                "{=lance_merc_player_cancel}I changed my mind, perhaps another time", () =>
                {
                    return HasEnoughGold(PartyBase.MainParty);
                },
                null
            );
            starter.AddPlayerLine(
                "lance_mercenary_hire_cant_afford",
                "lance_mercenary_hire",
                "close_window",
                "{=n5BGNLrc}That sounds good. But I can't afford any more men right now.", () =>
                {
                    return !HasEnoughGold(PartyBase.MainParty);
                },
                null
            );

            starter.AddDialogLine(
                "lance_mercenary_hire_tier_low",
                "lance_mercenary_hire_response",
                "close_window",
                "{DISMISSAL}",
                () =>
                {
                    if (Clan.PlayerClan.Tier != 0) return false;
                    _temporaryFinishedBusinessWithMerc = true;
                    var text = MercenaryLanceDialogs.GetHireDismissalDialog();
                    GameTexts.SetVariable("DISMISSAL", text);
                    return true;
                }, null
            );

            starter.AddDialogLine(
                "lance_mercenary_hire_limit",
                "lance_mercenary_hire_response",
                "lance_recruit_mercenaries_talk",
                "{=lance_mercenary_hire_limit}You already command more lances than you can keep fed and paid. I won’t chain my men to a sinking ship.",
                () =>
                {
                    return !CanHireNewMercenaryLance();
                },
                null
            );
            starter.AddDialogLine(
                "lance_mercenary_refill_response",
                "lance_mercenary_refill_response",
                "lance_mercenary_refill_player_response",
                "{=lance_mercenary_refill_yes}Coin for steel, steel for blood. pay {TOTAL_COST}{GOLD_ICON}, and I will send {AMOUNT} troops to your {TEMPLATE_NAME}",
                () =>
                {
                    var lanceChosen = (MercenaryLanceData)PartyBase.MainParty.Lances().Where(l => l is MercenaryLanceData && l.LanceRoster.Count < l.TotalManCount).GetRandomElementInefficiently();
                    CalculateNewTroopsToRefill(lanceChosen);
                    
                    GameTexts.SetVariable("AMOUNT", _tempRefillData!.AmountToRefill);
                    GameTexts.SetVariable("TEMPLATE_NAME", _tempRefillData.LanceData.TroopsTemplate.Name);
                    GameTexts.SetVariable("TOTAL_COST", _tempRefillData.TotalCost);
                    return true;
                }, null
            );
            starter.AddPlayerLine(
                "lance_mercenary_refill_player_response_yes",
                "lance_mercenary_refill_player_response",
                "close_window",
                "{=lance_recruitment_take}I will take them",
                null,
                () =>
                {
                    _lastRequestTimes[PlayerEncounter.EncounterSettlement.StringId] = CampaignTime.Now;
                    _temporaryFinishedBusinessWithMerc = true;
                    _tempRefillData!.LanceData.LanceRoster.Add(_tempRefillData.RefillTroopRoster);
                    PartyBase.MainParty.MemberRoster.Add(_tempRefillData.RefillTroopRoster);
                    GiveGoldAction.ApplyBetweenCharacters(PartyBase.MainParty.Owner, null, _tempRefillData.TotalCost, false);
                }
            );
            starter.AddPlayerLine(
                "lance_mercenary_refill_player_response_no",
                "lance_mercenary_refill_player_response",
                "close_window",
                "{=lance_options_no}I changed my mind",
                null, null
            );
        }
        public void CalculateNewTroopsToRefill(MercenaryLanceData data)
        {
            var amountToRefill = data.TotalManCount - data.LanceRoster.TotalManCount;
            TroopRoster refillTroopRoster = TroopRoster.CreateDummyTroopRoster();
            LanceModelUtils.RecruitNTroopsToRoster(amountToRefill, refillTroopRoster, data.TroopsTemplate.TroopsTemplate);
            int cost = 0;
            foreach(var troop in refillTroopRoster.GetTroopRoster())
                cost += troop.Character.TroopWage * troop.Number * BuyMercenaryMultiplier;
            _tempRefillData = new(data, refillTroopRoster, amountToRefill, cost);
        }
        private bool CanHireNewMercenaryLance() => 
            PartyBase.MainParty.Lances().Count < Campaign.Current.Models.LanceModel().MaxLancesForParty(PartyBase.MainParty).RoundedResultNumber;

        
        private TroopRoster CreateLanceTroops(int size, Lance lance)
        {
            TroopRoster mercenaryRoster = TroopRoster.CreateDummyTroopRoster();
            LanceModelUtils.RecruitNTroopsToRoster(size, mercenaryRoster, lance.TroopsTemplate);
            return mercenaryRoster;
        }
        private bool HasEnoughGold(PartyBase party)
        {
            return party.Owner.Gold >= _tempMercData!.Cost;
        }
        public void HireMercenaryLance(PartyBase party, Settlement settlement, TroopRoster mercenaryTroops)
        {
            GiveGoldAction.ApplyBetweenCharacters(party.Owner, null, _tempMercData!.Cost, false);
            var lancesBehavior = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
            int lanceSize = GetLanceSizeFromClan(Clan.PlayerClan);
            var name = GetLanceName(settlement, _tempMercData!.Template);
            var lanceData = new MercenaryLanceData(settlement.StringId, mercenaryTroops, name.ToString(), lanceSize, _tempMercData.Template, party.Owner.StringId);
            lancesBehavior.AddLanceToParty(party, lanceData);
            _mercData.Add(lanceData);
            _lastRequestTimes[settlement.StringId] = CampaignTime.Now;
        }
        public IEnumerable<Lance> GetPossibleTemplates(Settlement settlement)
        {
            return LanceTemplateManager.Instance.GetLances(settlement.Culture.StringId, LanceTemplateOriginType.Mercenary);
        }
        public TextObject GetLanceName(Settlement settlement, Lance lance)
        {
            var text = GameTexts.FindText("str_mercenary_lance_name", settlement.Culture.StringId);
            if (text.Value.Contains("ERROR"))
                text = GameTexts.FindText("str_mercenary_lance_name", "base");
            GameTexts.SetVariable("TEMPLATE_NAME", lance.Name);
            GameTexts.SetVariable("SETTLEMENT_NAME", settlement.Name);
            return text;
        }
        private void TryAddMercenaryToTavern(Dictionary<string, int> unusedUsablePointCount)
        {
            Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
            if (!settlement.IsTown
                || CampaignMission.Current == null
                || CampaignMission.Current.Location.StringId != "tavern")
                return;
            var settlementId = settlement.StringId;
            var now = CampaignTime.Now;
            if (_lastRequestTimes.TryGetValue(settlementId, out var lastTime) && now - lastTime < CampaignTime.Days(CooldownBeforeMercenaryIsInTownInDays))
                return;
            bool hasMercenary = _townsWithMercenaries.TryGetValue(settlementId, out var mercLeavesAt) && mercLeavesAt > now;
            if (!hasMercenary && ChanceToSpawnMercenary(settlement) < random.NextFloat())
            {
                _lastRequestTimes[settlementId] = now;
                return;
            }
            var possibleLances = GetPossibleTemplates(settlement).GetRandomElementInefficiently();
            _townsWithMercenaries[settlementId] = CampaignTime.DaysFromNow(CooldownBeforeMercenaryIsInTownInDays);
            var lanceSize = GetLanceSizeFromClan(Clan.PlayerClan);
            TroopRoster mercenaryTroops = CreateLanceTroops(lanceSize, possibleLances);
            var mercenaryLeader = mercenaryTroops.GetCharacterAtIndex(0);
            _tempMercData = new(mercenaryLeader, possibleLances, mercenaryTroops, CalculateLanceCost(mercenaryTroops));
            var locationCharacter = CreateMercenaryLeader(mercenaryLeader, LocationCharacter.CharacterRelations.Neutral);
            CampaignMission.Current.Location.AddCharacter(locationCharacter);
        }

        private int CalculateLanceCost(TroopRoster mercenaryTroops)
        {
            int cost = 0;
            foreach(var troop in mercenaryTroops.GetTroopRoster())
            {
                cost += troop.Character.TroopWage * troop.Number * BuyMercenaryMultiplier;
            }
            return cost;
        }

        private static LocationCharacter CreateMercenaryLeader(CharacterObject character, LocationCharacter.CharacterRelations relation)
        {
            Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
            int minValue = 30;
            int maxValue = 40;
            AgentData agentData = new AgentData(new SimpleAgentOrigin(character, -1, null, default)).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(minValue, maxValue));
            return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "spawnpoint_mercenary", true, relation, null, true, false, null, false, false, true, null, false);
        }
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("mercData", ref _mercData);
            dataStore.SyncData("GetMercenaries_LastRequestTimes", ref _lastRequestTimes);
            dataStore.SyncData("GetMercenaries_TownsWithMercenaries", ref _townsWithMercenaries);
        }
    }
}
