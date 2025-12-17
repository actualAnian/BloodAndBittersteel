using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.BlackfyreRebellion
{
    public static class RebellionConfig
    {
        public const string RebellionLeader = "clan_empire_north_2";
        public static readonly List<string> RebellionSupporterClans = new()
        {
            "clan_empire_north_4",
            "clan_empire_north_9",
            "clan_battania_2",
            "clan_battania_3",
            "clan_battania_7",
            "clan_battania_4",
            "clan_battania_5",
            "clan_battania_6",
            "clan_empire_west_2",
            "clan_empire_west_4",
            "clan_empire_west_3",
            "clan_aserai_1",
            "clan_empire_south_2",
            "clan_empire_south_3",
            "clan_empire_south_4",
            "clan_empire_south_5",
            "clan_empire_south_6",
            "clan_empire_south_7",
            "clan_empire_south_8",
            "clan_vlandia_2",
            "clan_vlandia_3",
            "clan_vlandia_4",
            "clan_vlandia_5",
            "clan_vlandia_6",
        };
        public static readonly List<string> LoyalistFactions = new()
        {
            "empire",
            "empire_w",
            "empire_s"
        };
        public static readonly List<string> LoyalistVassalsAtGameStart = new()
        {
            "sturgia",
            "battania",
            "vlandia",
            "aserai",
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
