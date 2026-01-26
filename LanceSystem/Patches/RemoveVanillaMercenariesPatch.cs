using HarmonyLib;
using static TaleWorlds.CampaignSystem.CampaignBehaviors.RecruitmentCampaignBehavior;

namespace LanceSystem.Patches
{
    [HarmonyPatch(typeof(TownMercenaryData), "ChangeMercenaryType")]
    public static class RemoveVanillaMercenariesPatch
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}
