using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;

namespace LanceSystem.Dialogues
{
    public class DisbandedLanceDialogs
    {
        public static void AddDisbandedLanceDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine(
                "disbanded_lance_start_not_owner",
                "start",
                "disbanded_lance_response",
                "{=dlp01}These men are no longer bound by the lance, my lord. Their service is done, and they now make their way home with what coin and honor they earned.",
                () =>
                {
                    return PlayerEncounter.EncounteredParty.MobileParty?.PartyComponent is DisbandedLancePartyComponent && PlayerEncounter.EncounteredParty.Owner != Hero.MainHero;
                },
                null
            );
            starter.AddDialogLine(
                "disbanded_lance_start_owner",
                "start",
                "disbanded_lance_owner_response",
                "{=dlp02}We stand ready, my lord. The lance was dismissed, but the men still look to you for command.",
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
                "{=dlp03}The lance rides with me again. Gather the men and fall in.",
                null, null
            );

            starter.AddDialogLine(
                "disbanded_lance_rejoin_confirm",
                "disbanded_lance_rejoin_response",
                "close_window",
                "{=dlp04}At once, my lord. The banner will be raised and the men will follow.",
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
                "{=dlp05}Stand aside or be cut down. I will not be gainsaid.",
                static () =>
                {
                    return PlayerEncounter.EncounteredParty.Owner != Hero.MainHero;
                },
                null
            );
            starter.AddDialogLine(
                "disbanded_lance_threat_reply",
                "disbanded_lance_threat_response",
                "disbanded_lance_confirm_attack",
                "{=dlp06}Choose your words carefully. To strike us is to strike the realm itself. Such blood will not be forgotten.",
                null,
                null
            );
            starter.AddPlayerLine(
                "disbanded_lance_attack_confirm",
                "disbanded_lance_confirm_attack",
                "close_window",
                "{=dlp07}Then let steel decide.",
                null,
                () =>
                {
                    //PlayerEncounter.Current.IsEnemy = true;
                    //PlayerEncounter.StartBattle();
                }
            );
            starter.AddPlayerLine(
                "disbanded_lance_leave",
                "disbanded_lance_response",
                "close_window",
                "{=dlp08}Very well. Go in peace.",
                null, () => { PlayerEncounter.LeaveEncounter = true; }

            );
            starter.AddPlayerLine(
                "lance_owner_leave",
                "disbanded_lance_owner_response",
                "close_window",
                "{=dlp09} Go to your homes and families",
                null, () => { PlayerEncounter.LeaveEncounter = true; }
            );
        }
    }
}
