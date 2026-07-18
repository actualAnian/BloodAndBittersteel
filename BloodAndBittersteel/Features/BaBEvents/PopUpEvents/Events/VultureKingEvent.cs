using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static BloodAndBittersteel.Features.BaBEvents.BaBEventTypes;
using RebellionConfig = BloodAndBittersteel.Features.BlackfyreRebellion.RebellionConfig;

namespace BloodAndBittersteel.Features.BaBEvents.PopUpEvents.Events
{
    public class VultureKingEvent
    {
        public const string StringId = "vulture_king";
        public const string ImageStringId = "test";
        public static readonly TextObject TitleText = new("{=bab_vulture_king_title}The Vulture King");
        public static readonly TextObject Description = new(
            "{=bab_vulture_king_desc}" +
            "{DORNE_KING} With the failure to curb the Blackfyre Rebellion, seeing the Prince of Dorne as a weak ruler, " +
            "having submitted fully to the Iron Throne and putting the interests of the Iron Throne over the people of Dorne " +
            "have declared a new Vulture King and have rose in open rebellion against the Prince's forces " +
            "putting aside their petty differences aside for the future of Dorne.");
        public static readonly List<string> ClansJoiningNewDornishKingdom = new()
        {
            "DORNE_12",
            "DORNE_9",
            "DORNE_19",
            "DORNE_18",
            "DORNE_17",
            "DORNE_16",
            "DORNE_20",
        };
        public const string DorneKingdomId = "Dorne";
        public static readonly List<string> KingdomsAtWarWithNewKingdom = new()
        {
            DorneKingdomId,
            "Crownlands",
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

            if (_random.NextDouble() > 0.9)
                return true;
            int dayOfYear = CampaignTime.Now.GetDayOfYear;
            if (CampaignTime.DaysInYear / dayOfYear > 0.5) 
                return true;
            return false;
        }

        public static void Consequence()
        {
            var dorneKingdom = Kingdom.All.FirstOrDefault(k => k.StringId == DorneKingdomId);
            var dorneLeader = dorneKingdom.Leader.Name;
            GameTexts.SetVariable("DORNE_KING", dorneLeader);
            var leaderClan = Clan.FindFirst(c => c.StringId == "DORNE_20");
            if (leaderClan == null)
                return;

            Kingdom newKingdom = Kingdom.CreateKingdom("bab_dornish_rebels");
            newKingdom.InitializeKingdom(
                new TextObject("{=bab_dorne_king_name}Vulture King's Realm"),
                new TextObject("{=bab_dorne_king_informal}Dornish Rebels"),
                leaderClan.Culture,
                RebellionConfig.RebellionBanner,
                1,
                1,
                leaderClan.HomeSettlement,
                new TextObject("{=bab_dorne_king_wiki}A new Dornish kingdom led by House Wyl"),
                new TextObject("{=bab_dorne_king_title}Dornish Rebellion"),
                new TextObject("{=bab_dorne_king_ruler}Lord Paramount")
            );
            ChangeKingdomAction.ApplyByCreateKingdom(leaderClan, newKingdom, false);

            foreach (var clanStringId in ClansJoiningNewDornishKingdom)
            {
                var clan = Clan.FindFirst(c => c.StringId == clanStringId);
                if (clan != null && !clan.IsEliminated)
                    ChangeKingdomAction.ApplyByJoinToKingdom(clan, newKingdom, default, false);
            }

            foreach (var kingdomStringId in KingdomsAtWarWithNewKingdom)
            {
                var kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == kingdomStringId);
                if (kingdom != null && !kingdom.IsEliminated)
                    FactionManager.DeclareWar(newKingdom, kingdom);
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
                CampaignTime.Never,
                Condition,
                Consequence);
        }
    }
}
