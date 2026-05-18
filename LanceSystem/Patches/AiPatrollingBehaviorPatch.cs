using HarmonyLib;
using LanceSystem.CampaignBehaviors;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace LanceSystem.Patches
{
    [HarmonyPatch(typeof(AiPatrollingBehavior), "GetDistanceScoreForLandPatrolling")]
    public static class AiPatrollingBehaviorPatch
    {
        static LancesCampaignBehavior? _behavior;
        private static bool HasAvailableLances(MobileParty party)
        {
            _behavior ??= Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
            return party.Owner.MapFaction.Settlements.Any(f => _behavior.DoesSettlementHaveFreeLances(f));
        }
        private static bool Prefix(Settlement targetSettlement, MobileParty mobileParty, float distanceToFurthestAllySettlementToFactionMidSettlement, out float bestDistanceScore)
        {
            float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty.MapFaction.FactionMidSettlement, targetSettlement, isFromPort: false, isTargetingPort: false, mobileParty.NavigationCapability);
            float num = ((distanceToFurthestAllySettlementToFactionMidSettlement != 0f) ? (distance / distanceToFurthestAllySettlementToFactionMidSettlement) : 0.5f);
            float num2 = MBMath.Map((float)0f, 0f, 1f, 0.2f, 0.8f);
            if (mobileParty.PartySizeRatio >= num2 || HasAvailableLances(mobileParty))
                bestDistanceScore = MBMath.Map(0.8f - (mobileParty.PartySizeRatio - num2), 0f, 0.8f, 0.2f, 1f);
            else
                bestDistanceScore = 0f;
            return false;
        }

    }
}
