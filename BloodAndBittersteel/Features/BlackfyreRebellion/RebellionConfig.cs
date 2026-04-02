using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BlackfyreRebellion
{
    public static class RebellionConfig
    {
        public const string RebellionLeader = "Blackfyre_1";
        public static readonly List<string> RebellionSupporterClans = new()
        {
            "Westerlands_10",
            "Westerlands_11",
            "Westerlands_25",
            "Crownlands_13",
            "Crownlands_10",
            "Crownlands_11",
            "Crownlands_22",
            "Crownlands_21",
            "Crownlands_25",
            "REACH_13",
            "REACH_12",
            "REACH_11",
            "REACH_10",
            "REACH_2",
            "REACH_33",
            "REACH_34",
            "REACH_32",
            "Stormlands_21",
            "Stormlands_15",
            "Stormlands_28",
            "Stormlands_27",
            "Stormlands_26",
            "Stormlands_24",
            "Stormlands_16",
            "Stormlands_19",
            "Stormlands_20",
            "DORNE_12",
            "Vale_6",
            "Vale_22",
            "Vale_23",
            "Riverlands_6",
            "Riverlands_7",
            "Riverlands_15",
            "Riverlands_9",
            "Riverlands_8",
            "Riverlands_13",
            "Riverlands_12",
        };
        public static readonly List<string> LoyalistFactions = new()
        {
            "Vale",
            "Westerlands",
            "Dorne",
            "Reach",
            "Stormlands",
            "Riverlands",
        };
        public static readonly List<string> LoyalistVassalsAtGameStart = new()
        {
            "Vale",
            "North",
            "Reach",
            "Riverlands",
            "Westerlands",
            "Stormlands",
            "Dorne",
        };
        public static readonly Banner RebellionBanner = new();
        public const string RebellionFactionStringId = "bab_blackfyre_rebels";
        public const string RebellionFactionName = "{=bab_bf_rebellion_name}Blackfyre Rebellion";
        public const string RebellionFactionInformalName = "{=bab_bf_rebellion_informal_name}Blackfyre Rebellion";
        public const string RebellionFactionEncyclopediaText = "{=bab_bf_rebellion_text}The Blackfyre Rebellion was the first great civil war of the Targaryen dynasty, born from questions of succession and stoked by the ambitions of Daemon Blackfyre, the legitimized son of King Aegon IV. Upon Aegon’s death, the Iron Throne passed to Daeron II, yet many lords doubted his parentage and rallied instead behind Daemon, who claimed both royal blood and the Valyrian blade Blackfyre itself. The realm split between those loyal to King Daeron and those who believed Daemon the truer heir.";
        public const string RebellionFactionEncyclopediaTitle = "{=bab_bf_rebellion_title}The Blackfyre Rebellion";
        public const string RebellionFactionEncyclopediaRulerTitle = "{=bab_bf_ruler_name}Pretender King Daemon Blackfyre";
        public const int RelationGainOnJoinKingdom = 50;
        public static CampaignTime RebellionStartTime => Campaign.Current.Models.CampaignTimeModel.CampaignStartTime+ CampaignTime.Weeks(Campaign.Current.Models.CampaignTimeModel.WeeksInSeason * 6);
    }
}
