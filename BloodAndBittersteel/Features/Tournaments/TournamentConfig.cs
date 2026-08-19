using BloodAndBittersteel.Library.RuleEngine;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.Tournaments
{
    public static class TournamentConfig
    {
        private readonly static Dictionary<string, List<string>> ClanItems = new()
        {
            [Globals.ClanLannisterId] = new() { "grain", "grain" },
            [Globals.ClanStarkId] = new() { "grain", "grain" },
        };
        private readonly static Dictionary<string, List<string>> CultureItems = new()
        {
            [Globals.IronbornCultureId] = new() { "grain", "grain" },
        };
        private static readonly Rule<Town, ItemObject> BelongsToClan = new(
            matches: town => ClanItems.ContainsKey(town.OwnerClan.StringId),
            resolve: town => MBObjectManager.Instance.GetObject<ItemObject>(ClanItems[town.OwnerClan.StringId].GetRandomElement()));
        private static readonly Rule<Town, ItemObject> BelongsToCulture = new(
            matches: town => CultureItems.ContainsKey(town.OwnerClan.Culture.StringId),
            resolve: town => MBObjectManager.Instance.GetObject<ItemObject>(CultureItems[town.OwnerClan.Culture.StringId].GetRandomElement()));
        private static readonly Rule<Town, ItemObject> Fallback = new(
            matches: town => true,
            resolve: town => MBObjectManager.Instance.GetObject<ItemObject>("grain"));
        public static OrderedRuleEngine<Town, ItemObject> GetBasicTournamentRewardItem = new(new List<Rule<Town, ItemObject>>()
        {
            BelongsToClan,
            BelongsToCulture,
            Fallback
        });
    }
}
