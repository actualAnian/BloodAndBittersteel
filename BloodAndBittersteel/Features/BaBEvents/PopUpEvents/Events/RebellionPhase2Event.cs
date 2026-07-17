using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Localization;
using static BloodAndBittersteel.Features.BaBEvents.BaBEventTypes;
using RebellionConfig = BloodAndBittersteel.Features.BlackfyreRebellion.RebellionConfig;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents.Events
{
    public class RebellionPhase2Event
    {
        public const string StringId = "rebellion_phase2";
        public const string ImageStringId = "test";
        public static readonly TextObject TitleText = new("{=bab_rebel_phase2_title}The King who Bore the Sword");

    public static readonly TextObject Description = new TextObject(
        "{=bab_rebel_phase2_desc}" +
        "Some conflicts have the courtesy of ending quickly. This is not one of those times. The rebellion has raged for years, with devastation and brutality trailing its ever-changing front lines" +
        "As instability breeds more instability, many once-staunch loyalists have begun to question their allegiances. Surely being the one to tip the scales would vastly increase their houses's standing in the new order." +
        "With the trade grinding to a halt, the rebellion is no longer a contained event. It has to be stopped at all costs. Even the most cautios Lord Paramounts, have called their banners in support of the Iron Throne. " +
        "The realm is now at total war.");
        public static readonly List<string> ClansJoiningDaemon = new()
        {
            "Riverlands_19",
            "Riverlands_23",
            "Riverlands_25",
            "Riverlands_26",
            "Riverlands_28",
            "REACH_28",
            "REACH_29",
            "REACH_30",
        };

        public static readonly List<string> KingdomsAllyingDaemon = new()
        {
            "Ironborn",
        };

        public static readonly List<string> RemainingKingdomsJoiningCrown = new()
        {
            "Stormlands",
            "Reach",
            "Riverlands",
        };
        public static BaBPopupEvent Instance => _instance ??= CreateInstance();
        private static BaBPopupEvent? _instance;
        private static Random _random = new();

        private static BaBPopupEvent CreateInstance()
        {
            return new BaBPopupEvent(
                StringId,
                OnDailyTick,
                1f,
                ImageStringId,
                TitleText,
                Description,
                CampaignTime.Never,
                Fire,
                CheckCondition);
        }

        public static bool CheckCondition()
        {
            var daemonClan = Clan.FindFirst(c => c.StringId == RebellionConfig.RebellionLeader);
            if (daemonClan == null || daemonClan.Leader?.IsDead == true)
                return false;

            int year = CampaignTime.Now.GetYear;
            if (year >= 197)
                return true;

            int dayOfYear = CampaignTime.Now.GetDayOfYear;
            if (CampaignTime.DaysInYear / dayOfYear > 0.9)
                return _random.NextDouble() > 0.9;

            return false;
        }

        public static void Fire()
        {
            var daemonKingdom = Kingdom.All.First(k => k.StringId == RebellionConfig.RebellionFactionStringId);
            var crownlands = Kingdom.All.First(k => k.StringId == RebellionConfig.CrownlandsKingdomStringId);

            foreach (var clanStringId in ClansJoiningDaemon)
            {
                var clan = Clan.FindFirst(c => c.StringId == clanStringId);
                if (clan != null && !clan.IsEliminated)
                    ChangeKingdomAction.ApplyByJoinToKingdom(clan, daemonKingdom, default, false);
            }

            foreach (var kingdomStringId in KingdomsAllyingDaemon)
            {
                var kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == kingdomStringId);
                if (kingdom != null && !kingdom.IsEliminated)
                {
                    FactionManager.DeclareWar(daemonKingdom, kingdom);
                }
            }

            foreach (var kingdomStringId in RemainingKingdomsJoiningCrown)
            {
                var kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == kingdomStringId);
                if (kingdom != null && !kingdom.IsEliminated)
                {
                    Campaign.Current.GetCampaignBehavior<AllianceCampaignBehavior>().StartAlliance(kingdom, crownlands);
                }
            }
        }
    }
}
