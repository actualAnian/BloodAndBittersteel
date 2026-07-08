using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.NightsWatch
{
    public static class NightsWatchConfig
    {
        public static readonly List<string> KingdomsWhoCanForceToNightsWatch = new()
        {
            "Riverlands",
            "Westerlands",
            "Crownlands",
            "Stormlands",
            "Vale",
            "Dorne",
            "Reach",
            "North",
            "Ironborn"
        };
        public const int LordsInNightsWatchBeforeChanceReduction = 20;
        public const float ChanceReductionPerLordInNightsWatch = 0.05f;
        public const string NightsWatchFactionStringId = "vlandia";
        public static Kingdom NightsWatchKingdom => Kingdom.All.First(k => k.StringId == NightsWatchFactionStringId);
        public static int GetAmountOfLordsInNightsWatch()
        {
            int amount = 0;

            return amount;
        }
    }
}
