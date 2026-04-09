using Helpers;
using LanceSystem.CampaignBehaviors;
using LanceSystem.UI;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LanceSystem.Dialogues
{
    public class DisbandedLanceDialogs
    {
        static readonly Random _random = new();
        const float SurrenderStrengthRatio = 5f;
        const float SurrenderChance = 0.5f;
        static PartyScreenLogic? _logic;
        public static void AddDisbandedLanceDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine(
                "disbanded_lance_start_not_owner",
                "start",
                "disbanded_lance_response",
                "{=lance_disbanded_not_owner}These men are no longer bound by the lance, my lord. Their service is done, and they now make their way home with what coin and honor they earned.",
                () =>
                {
                    return PlayerEncounter.EncounteredParty?.MobileParty?.PartyComponent is DisbandedLancePartyComponent && PlayerEncounter.EncounteredParty.Owner != Hero.MainHero
                    && PlayerEncounter.EncounteredParty.MobileParty.Owner.CharacterObject != CharacterObject.OneToOneConversationCharacter;
                },
                null
            );
            starter.AddDialogLine(
                "disbanded_lance_start_owner",
                "start",
                "disbanded_lance_owner_response",
                "{=lance_disbanded_owner}We stand ready, my lord. The lance was dismissed, but the men still look to you for command.",
                () =>
                {
                    return PlayerEncounter.EncounteredParty?.MobileParty?.PartyComponent is DisbandedLancePartyComponent && PlayerEncounter.EncounteredParty.Owner == Hero.MainHero;
                },
                null
            );
            starter.AddPlayerLine(
                "lance_recruit_merc_player",
                "disbanded_lance_response",
                "lance_recruit_merc_player",
                "{RECRUIT_MERC_PLAYER}",
                static () =>
                {
                    var isCruel = Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0;
                    var text = isCruel ? "{=lance_player_ask_cruel}You’ve been cast loose. Better to profit than wander — serve me instead."
                    : "{=lancel_player_ask}You are free men now. Take service with me — for fair pay.";
                    GameTexts.SetVariable("RECRUIT_MERC_PLAYER", text);
                    return !Clan.PlayerClan.MapFaction.IsAtWarWith(PlayerEncounter.EncounteredMobileParty.MapFaction);
                },
                null);

            starter.AddDialogLine(
                "lance_recruit_agree",
                "lance_recruit_merc_player",
                "lance_recruit_agree",
                "{RECRUIT_MERC_RESPONSE}",
                static () =>
                {
                    string text = Clan.PlayerClan.Tier switch
                    {
                        0 or 1 => "{=lance_merc_accept_low}We’ve marched under worse. Pay us, and we’re yours.",
                        2 => "{=lance_merc_accept_mid}We’ve heard of you. For coin, we’ll follow your banner.",
                        _ => "{=lance_merc_accept_high}Gladly. We will serve under your command.",
                    };
                    GameTexts.SetVariable("RECRUIT_MERC_RESPONSE", text);
                    return Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>().CanRecruitDisbandedLanceAsMercenaries();
                },
                null);
            starter.AddDialogLine(
                "lance_recruit_refuse",
                "lance_recruit_merc_player",
                "close_window",
                "{RECRUIT_MERC_RESPONSE}",
                static () =>
                {
                    string text = Clan.PlayerClan.Tier switch
                    {
                        0 or 1 => "{=lance_merc_refuse_low}Find other fools. We’ll not follow you.",
                        2 => "{=lance_merc_refuse_mid}Not a banner we care to follow.",
                        _ => "{=lance_merc_refuse_high}you honour us my {?PLAYER.GENDER}lady{?}lord{\\?}, but we are weary after an arduous campaign, we must refuse.",
                    };
                    GameTexts.SetVariable("RECRUIT_MERC_RESPONSE", text);
                    var willRefuse = !Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>().CanRecruitDisbandedLanceAsMercenaries();
                    if (willRefuse)
                        PlayerEncounter.LeaveEncounter = true;
                    return willRefuse;
                }, null);


            starter.AddPlayerLine(
                "lance_recruit_agree_confirm",
                "lance_recruit_agree",
                "close_window",
                "{=lance_player_merc_accept}Good. Keep your discipline, and you’ll not regret this.",
                null, static () =>
                {
                    PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
                    partyState.PartyScreenMode = PartyScreenHelper.PartyScreenMode.Normal;
                    var partyScreenLogic = new PartyScreenLogic();
                    var otherParty = PlayerEncounter.EncounteredMobileParty;
                    TroopRoster leftMemberRoster = TroopRoster.CreateDummyTroopRoster();
                    leftMemberRoster.Add(PlayerEncounter.EncounteredMobileParty.MemberRoster);
                    PartyBase? leftOwnerParty = PlayerEncounter.EncounteredMobileParty.Party;
                    int leftPartyMembersSizeLimit = Math.Max(otherParty.Party.PartySizeLimit - otherParty.Party.NumberOfAllMembers, 0);
                    PartyPresentationDoneButtonDelegate OnDone =
                    (leftMemberRoster,
                        leftPrisonRoster,
                        rightMemberRoster,
                        rightPrisonRoster,
                        takenPrisonerRoster,
                        releasedPrisonerRoster,
                        isForced,
                        leftParty,
                        rightParty) =>
                    {
                        MercenaryTroopsBuyLogic.Instance.OnDone(leftParty);
                        return true;
                    };

                    PartyScreenLogicInitializationData initializationData = PartyScreenLogicInitializationData.CreateBasicInitDataWithMainParty(
                        leftMemberRoster,
                        TroopRoster.CreateDummyTroopRoster(),
                        PartyScreenLogic.TransferState.Transferable,
                        PartyScreenLogic.TransferState.NotTransferable,
                        PartyScreenLogic.TransferState.Transferable,
                        new IsTroopTransferableDelegate(PartyScreenHelper.TroopTransferableDelegate),
                        partyState.PartyScreenMode,
                        leftOwnerParty,
                        otherParty.Name,
                        new TextObject("{=uQgNPJnc}Manage Troops", null),
                        null,
                        leftPartyMembersSizeLimit,
                        0, OnDone, null, null, null, null, false, false, false, false, 0);
                    partyScreenLogic.Initialize(initializationData);
                    partyState.PartyScreenLogic = partyScreenLogic;
                    _logic = partyScreenLogic;
                    Game.Current.GameStateManager.PushState(partyState, 0);
                    MercenaryTroopsBuyLogic.Instance.Init(otherParty.MemberRoster, partyScreenLogic);
                    PartyCharacterVM.OnTransfer += (troop, index, amount, fromSide) => MercenaryTroopsBuyLogic.Instance.OnTroopTransfer(troop.Character, amount, fromSide);
                    partyScreenLogic.AfterReset += (logic, fromCancel) => MercenaryTroopsBuyLogic.Instance.Reset();
                    PlayerEncounter.LeaveEncounter = true;
                });

            starter.AddPlayerLine(
                "disbanded_lance_rejoin",
                "disbanded_lance_owner_response",
                "disbanded_lance_rejoin_response",
                "{=lance_disbanded_rejoin}The lance rides with me again. Gather the men and fall in.",
                null, null
            );

            starter.AddDialogLine(
                "disbanded_lance_rejoin_confirm",
                "disbanded_lance_rejoin_response",
                "close_window",
                "{=lance_disbanded_rejoin_confirm}At once, my lord. The banner will be raised and the men will follow.",
                null,
                () =>
                {
                    var behavior = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
                    behavior.ReAddLanceToPlayerParty(PlayerEncounter.EncounteredParty.MobileParty);
                    PlayerEncounter.LeaveEncounter = true;
                }
            );
            starter.AddPlayerLine(
                "disbanded_lance_threaten",
                "disbanded_lance_response",
                "disbanded_lance_threat_response",
                "{AGGRESS_TEXT}",
                static () =>
                {
                    string text;
                    var isAtWar = Clan.PlayerClan.MapFaction.IsAtWarWith(PlayerEncounter.EncounteredParty.MapFaction);
                    var isCruel = Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0;
                    text = isAtWar
                        ? isCruel ? "{=lance_disbanded_yield_war_cruel}You crawl home like beaten dogs. I may cut you down for the sport of it."
                        : "{=lance_disbanded_yield_war}Yield your arms and no harm will come to you. I have no wish to spill blood needlessly."
                        : isCruel ? "{=lance_disbanded_yield_peace_cruel}You thought the road was safe. You were wrong."
                        : "{=lance_disbanded_yield_peace}What have we got here. Coin, steel — I’ll take what I please.";
                    GameTexts.SetVariable("AGGRESS_TEXT", text);
                    return PlayerEncounter.EncounteredParty.Owner != Hero.MainHero;
                },
                null
            );
            starter.AddDialogLine(
                "disbanded_lance_threat_surrender",
                "disbanded_lance_threat_response",
                "close_window",
                "{=lance_enemy_surrender}Let us stop senseless bloodshed. We yield.",
                static () =>
                {
                    var isAtWar = Clan.PlayerClan.MapFaction.IsAtWarWith(PlayerEncounter.EncounteredParty.MapFaction);
                    if (!isAtWar) return false;
                    float playerStrength = MobileParty.MainParty.Party.CalculateCurrentStrength();
                    float enemyStrength = MobileParty.ConversationParty.Party.CalculateCurrentStrength();
                    var playerFriends = new List<MobileParty>();
                    var enemyFriends = new List<MobileParty>();
                    PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(playerFriends, enemyFriends);
                    foreach (var party in enemyFriends)
                        enemyStrength += party.Party.CalculateCurrentStrength();
                    if (playerStrength > enemyStrength * SurrenderStrengthRatio
                    && _random.NextFloat() < SurrenderChance) return true;
                    return false;
                },
                () =>
                {
                    if (PlayerEncounter.Battle == null)
                    {
                        PlayerEncounter.StartBattle();
                    }
                    PlayerEncounter.Battle!.SetOverrideWinner(PlayerEncounter.Battle.PlayerSide);
                    PlayerEncounter.EnemySurrender = true;
                    return;
                }
            );
            starter.AddDialogLine(
                "disbanded_lance_threat_reply",
                "disbanded_lance_threat_response",
                "disbanded_lance_confirm_attack",
                "{AGGRESS_TEXT}",
                static () =>
                {
                    string text;
                    var isAtWar = Clan.PlayerClan.MapFaction.IsAtWarWith(PlayerEncounter.EncounteredParty.MapFaction);
                    if (!isAtWar)
                    {
                        text = "Choose your words carefully. To strike us is to strike the realm itself. Such blood will not be forgotten.";
                        GameTexts.SetVariable("AGGRESS_TEXT", text);
                        return true;
                    }
                    float playerStrength = MobileParty.MainParty.Party.CalculateCurrentStrength();
                    float enemyStrength = MobileParty.ConversationParty.Party.CalculateCurrentStrength();
                    var playerFriends = new List<MobileParty>();
                    var enemyFriends = new List<MobileParty>();
                    PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(playerFriends, enemyFriends);
                    foreach (var party in enemyFriends)
                        enemyStrength += party.Party.CalculateCurrentStrength();
                    if (playerStrength > enemyStrength * SurrenderStrengthRatio) text = "{=lance_disbanded_fight_fatalism}You may claim our lives, but not our honor. We will not yield.";
                    else text = "{=lance_disbanded_fight}You will find no easy victory here. We stand as men who have not yet spent their last breath.";
                    GameTexts.SetVariable("AGGRESS_TEXT", text);
                    return true;
                },
                null
            );
            starter.AddPlayerLine(
                "disbanded_lance_attack_confirm",
                "disbanded_lance_confirm_attack",
                "close_window",
                "{=lance_attack_starts}Then let steel decide.",
                null,
                () =>
                {
                    var party = PlayerEncounter.EncounteredParty;
                    if (!Clan.PlayerClan.MapFaction.IsAtWarWith(party.MapFaction))
                        DeclareWarAction.ApplyByPlayerHostility(Clan.PlayerClan.MapFaction, party.MapFaction);
                }
            );
            starter.AddPlayerLine(
                "disbanded_lance_attack_leave",
                "disbanded_lance_confirm_attack",
                "close_window",
                "{=VbUnP1M5}Very well. For now, go in peace.",
                null, () => { PlayerEncounter.LeaveEncounter = true; }
            );

            starter.AddPlayerLine(
                "disbanded_lance_leave",
                "disbanded_lance_response",
                "close_window",
                "{=VbUnP1M5}Very well. For now, go in peace.",
                null, () => { PlayerEncounter.LeaveEncounter = true; }
            );
            starter.AddPlayerLine(
                "lance_owner_leave",
                "disbanded_lance_owner_response",
                "close_window",
                "{=lance_disbanded_go} Go to your homes and families",
                null, () => { PlayerEncounter.LeaveEncounter = true; }
            );
        }
    }
}
