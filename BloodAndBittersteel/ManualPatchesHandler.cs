using BloodAndBittersteel.Features.LanceSystem.Patches;
using HarmonyLib;
using System.Reflection;

namespace BloodAndBittersteel
{
    public static class ManualPatchesHandler
    {
        static bool _hasRun = false;
        public static void TryRunManualPatches(Harmony harmony)
        {
            if (_hasRun) return;
            _hasRun = true;
            RunRecruitmentPatches(harmony);
        }
        public static void RunRecruitmentPatches(Harmony harmony)
        {
#pragma warning disable BHA0003 // Type was not found
            MethodInfo originalMethod = AccessTools.Method("PlayerTownVisitCampaignBehavior:game_menu_town_recruit_troops_on_condition");
            harmony.Patch(originalMethod, prefix: new HarmonyMethod(typeof(PlayerTownVisitCampaignBehaviorPatch), nameof(PlayerTownVisitCampaignBehaviorPatch.Prefix)));
#pragma warning restore BHA0003 // Type was not found

        }
    }
}
