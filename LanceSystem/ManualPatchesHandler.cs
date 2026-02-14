using HarmonyLib;
using LanceSystem.Patches;
using LanceSystem.UI.UIPatches;
using System.Reflection;

namespace LanceSystem
{
    public static class ManualPatchesHandler
    {
        static bool _hasRun = false;
        public static void TryRunManualPatches(Harmony harmony)
        {
            if (_hasRun) return;
            _hasRun = true;
            RunRecruitmentPatches(harmony);
            RunUIPatches(harmony);
        }
#pragma warning disable BHA0003 // Type was not found
        public static void RunRecruitmentPatches(Harmony harmony)
        {
            MethodInfo townMethod = AccessTools.Method("PlayerTownVisitCampaignBehavior:game_menu_town_recruit_troops_on_condition");
            harmony.Patch(townMethod, prefix: new HarmonyMethod(typeof(PlayerTownVisitCampaignBehaviorPatch), nameof(PlayerTownVisitCampaignBehaviorPatch.Prefix)));
            MethodInfo villageMethod = AccessTools.Method("PlayerTownVisitCampaignBehavior:game_menu_recruit_volunteers_on_condition");
            harmony.Patch(villageMethod, prefix: new HarmonyMethod(typeof(PlayerTownVisitCampaignBehaviorPatch), nameof(PlayerTownVisitCampaignBehaviorPatch.Prefix)));
        }
        public static void RunUIPatches(Harmony harmony)
        {
            MethodInfo updateTroop = AccessTools.Method("PartyTradeVM:UpdateTroopData");
            harmony.Patch(updateTroop, prefix: new HarmonyMethod(typeof(PartyTradeVMPatch), nameof(PartyTradeVMPatch.Prefix)));
            MethodInfo upgradeTroop = AccessTools.Method("PartyCharacterVM:Upgrade");
            harmony.Patch(upgradeTroop, prefix: new HarmonyMethod(typeof(PartyCharacterVMPatch), nameof(PartyCharacterVMPatch.Prefix)));
            MethodInfo recruitTroop = AccessTools.Method("PartyCharacterVM:ExecuteRecruitTroop");
            harmony.Patch(recruitTroop, prefix: new HarmonyMethod(typeof(PartyCharacterVMRecruitPrisonerPatch), nameof(PartyCharacterVMRecruitPrisonerPatch.Prefix  )));
        }
    }
#pragma warning restore BHA0003 // Type was not found
}
