using BloodAndBittersteel.Features.FemaleLords;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace BloodAndBittersteel.Features.Tournaments
{
    [HarmonyPatch(typeof(FightTournamentGame), nameof(FightTournamentGame.CanBeAParticipant))]
    public class CanNpcsParticipatePatch // player participation is defined in BaBSettlementAccessModel.CanPlayerJoinTournament
    {
        public static void Postfix(CharacterObject character, bool considerSkills, ref bool __result)
        {
            if (__result == false) return;

            if (!FemaleLordsConfig.CanLeadParties(character))
            {
                __result = false;
                return;
            }
        }
    }
}
