using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.Tournaments
{
    [HarmonyPatch(typeof(FightTournamentGame), nameof(FightTournamentGame.CanBeAParticipant))]
    public class CanNpcsParticipatePatch // player participation is defined in BaBSettlementAccessModel.CanPlayerJoinTournament
    {
        public static void Postfix(CharacterObject character, bool considerSkills, ref bool __result)
        {
            if (__result == false) return;

            if (!TournamentConfig.CanParticipate(character))
            {
                __result = false;
                return;
            }
        }
    }
}
