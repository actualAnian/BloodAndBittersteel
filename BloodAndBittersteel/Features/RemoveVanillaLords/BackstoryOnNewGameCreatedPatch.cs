using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace BloodAndBittersteel.Features.RemoveVanillaLords
{
    [HarmonyPatch(typeof(BackstoryCampaignBehavior), "OnNewGameCreated")]
    public static class BackstoryOnNewGameCreatedPatch
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}
