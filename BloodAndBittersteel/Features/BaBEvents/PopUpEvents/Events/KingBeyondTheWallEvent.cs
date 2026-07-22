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
    public static class KingBeyondTheWallEvent
    {
        public const string StringId = "king_beyond_wall";
        public const string ImageStringId = "test";
        public static readonly TextObject TitleText = new("{=bab_king_beyond_title}King-Beyond-the-Wall");

        public static readonly TextObject Description = new("{=bab_king_beyond_description}" +
            "The woes of one kingdom are the fortune of another. " +
            "The Night's Watch has grown complacent, its castles undermanned and its vigilance broken by the troubles of the south." +
            "Behind it, fertile lands, that promise to stop the never ending hunger." +
            "That is the future, that Raymun Redbeard promised. That is what unites the once-feuding clans, under their King-Beyond-the-Wall. To claim the home they have so long been denied");
        public static readonly List<string> ClansJoiningFreeFolk = new()
        {
            "FREEFOLK_1",
            "FREEFOLK_2",
            "FREEFOLK_3",
            "FREEFOLK_4",
            "FREEFOLK_5",
            "FREEFOLK_6",
            "FREEFOLK_7",
        };

        public static readonly List<string> KingdomsAtWarWithFreeFolk = new()
        {
            "North",
        };

        public static readonly List<string> KingdomsAllyingFreeFolk = new()
        {
            Globals.SkagosKingdomId
        };

        private static readonly Random _random = new();

        public static bool Condition()
        {
            var daemonClan = Clan.FindFirst(c => c.StringId == RebellionConfig.RebellionLeader);
            if (daemonClan == null || daemonClan.Leader?.IsDead == true)
                return false;

            int year = CampaignTime.Now.GetYear;
            if (year < 197)
                return false;
            if (_random.NextDouble() > 0.9) return true;

            int dayOfYear = CampaignTime.Now.GetDayOfYear;
            if (CampaignTime.DaysInYear / dayOfYear > 0.2)
                return true;

            return false;
        }

        public static void Consequence()
        {
            var leaderClan = Clan.FindFirst(c => c.StringId == "FREEFOLK_1");
            if (leaderClan == null)
                return;

            Kingdom freeFolkKingdom = Kingdom.CreateKingdom("bab_free_folk");
            freeFolkKingdom.InitializeKingdom(
                new TextObject("{=bab_free_folk_name}Free Folk"),
                new TextObject("{=bab_free_folk_informal}Wildlings"),
                leaderClan.Culture,
                RebellionConfig.RebellionBanner,
                1,
                1,
                leaderClan.HomeSettlement,
                new TextObject("{=bab_free_folk_wiki}The united Free Folk under Raymun Redbeard"),
                new TextObject("{=bab_free_folk_title}King-Beyond-the-Wall"),
                new TextObject("{=bab_free_folk_ruler}King-Beyond-the-Wall")
            );
            ChangeKingdomAction.ApplyByCreateKingdom(leaderClan, freeFolkKingdom, false);

            foreach (var clanStringId in ClansJoiningFreeFolk)
            {
                var clan = Clan.FindFirst(c => c.StringId == clanStringId);
                if (clan != null && !clan.IsEliminated)
                    ChangeKingdomAction.ApplyByJoinToKingdom(clan, freeFolkKingdom, default, false);
            }

            foreach (var kingdomStringId in KingdomsAtWarWithFreeFolk)
            {
                var kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == kingdomStringId);
                if (kingdom != null && !kingdom.IsEliminated)
                    FactionManager.DeclareWar(freeFolkKingdom, kingdom);
            }

            foreach (var kingdomStringId in KingdomsAllyingFreeFolk)
            {
                var kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == kingdomStringId);
                if (kingdom != null && !kingdom.IsEliminated)
                    Campaign.Current.GetCampaignBehavior<AllianceCampaignBehavior>().StartAlliance(kingdom, freeFolkKingdom);
            }
        }
        [BaBEvent]
        private static BaBPopupEvent CreateEvent()
        {
            return new BaBPopupEvent(
                StringId,
                OnDailyTick,
                1f,
                ImageStringId,
                TitleText,
                Description,
                CampaignTime.Days(24),
                Condition,
                Consequence);
        }
    }
}
