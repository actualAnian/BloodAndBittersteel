using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace BloodAndBittersteel.Features.Jousting.Patches
{
    [HarmonyPatch(typeof(TournamentCampaignBehavior), "game_menu_tournament_join_current_game_on_consequence")]
    public class JoinJoustTournamentPatch
    {
        public static void Postfix()
        {
            var settlement = Settlement.CurrentSettlement;
            if (settlement?.Town == null) return;

            var tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(settlement.Town);
            if (tournamentGame is JoustTournamentGame && settlement.Owner != Hero.MainHero)
            {
                GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, -10000);
            }
        }
    }
}
