using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
namespace LanceSystem.SimpleFuzzySearch
{
    public static class FuzzySearchCommands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("try_fuzzy_search", "bab")]
        public static string TryFuzzySearch(List<string> args)
        {
            if (args.Count != 1) return "Usage: try_fuzzy_search <item_id>";
            string itemId = args != null && args.Count > 0 ? args[0] : "looter";
            var result = FuzzySearchManager.TrySearch<ItemObject>(itemId, item => item.Name.ToString());
            var resultString = $"Found {result.Count} results for query '{itemId}':\n";
            foreach (var searchResult in result)
            {
                resultString += $"- {searchResult.Item.Name}\n";
            }
            return resultString;
        }
    }
}
