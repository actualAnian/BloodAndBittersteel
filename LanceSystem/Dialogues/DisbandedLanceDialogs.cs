using LanceSystem.CampaignBehaviors;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace LanceSystem.Dialogues
{
    public class DisbandedLanceDialogs
    {
        static readonly Random _random = new();
        const float SurrenderStrengthRatio = 5f;
        const float SurrenderChance = 0.5f;
        public static void AddDisbandedLanceDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine(
                "disbanded_lance_start_not_owner",
                "start",
                "disbanded_lance_response",
                "{=lance_disbanded_not_owner}These men are no longer bound by the lance, my lord. Their service is done, and they now make their way home with what coin and honor they earned.",
                () =>
                {
                    return PlayerEncounter.EncounteredParty.MobileParty?.PartyComponent is DisbandedLancePartyComponent && PlayerEncounter.EncounteredParty.Owner != Hero.MainHero
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
                    return PlayerEncounter.EncounteredParty.MobileParty?.PartyComponent is DisbandedLancePartyComponent && PlayerEncounter.EncounteredParty.Owner == Hero.MainHero;
                },
                null
            );
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
