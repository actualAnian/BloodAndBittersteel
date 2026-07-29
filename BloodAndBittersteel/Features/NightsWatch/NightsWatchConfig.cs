using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.NightsWatch
{
    public static class NightsWatchConfig
    {
        public static readonly List<string> KingdomsWhoCanForceToNightsWatch = new()
        {
            Globals.RiverlandsKingdomId,
            Globals.WesterlandsKingdomId,
            Globals.CrownlandsKingdomStringId,
            Globals.StormlandsKingdomId,
            Globals.ValeKingdomId,
            Globals.DorneKingdomId,
            Globals.ReachKingdomId,
            Globals.NorthKingdomId,
            Globals.IronbornKingdomId
        };
        public const int LordsInNightsWatchBeforeChanceReduction = 20;
        public const float ChanceReductionPerLordInNightsWatch = 0.05f;
        public const float BaseChanceForAIToSendToNightsWatch = 0.1f;
        public const float BaseChanceForAIToAcceptPlayerOfferToJoinNightsWatch = 0.5f;
        public const float ChanceForRulerPerRelationPoint = -0.01f;
        public static Kingdom NightsWatchKingdom => Kingdom.All.First(k => k.StringId == Globals.NightsWatchKingdomStringId);
        public static Clan NightsWatchClanToJoin => NightsWatchKingdom.Clans.First(c => c.StringId == "NW_1");
        public static int GetAmountOfLordsInNightsWatch => NightsWatchKingdom.AliveLords.Count;
    }
}
