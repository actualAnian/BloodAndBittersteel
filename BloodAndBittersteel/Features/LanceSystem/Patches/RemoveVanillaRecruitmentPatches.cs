
using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameMenus;

namespace BloodAndBittersteel.Features.LanceSystem.Patches
{
    public static class PlayerTownVisitCampaignBehaviorPatch
    {
        public static bool Prefix(MenuCallbackArgs args, ref bool __result)
        {
            args.IsEnabled = false;
            args.Tooltip = new("{=bab_disabled_recruitment}The recruitment system is disabled, talk to the notables");
            __result = true;
            return false;
        }
    }
}
