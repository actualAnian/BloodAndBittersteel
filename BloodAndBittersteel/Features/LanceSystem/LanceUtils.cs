using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using static BloodAndBittersteel.Features.LanceSystem.LancesCampaignBehavior;

namespace BloodAndBittersteel.Features.LanceSystem
{
    public static class LanceUtils
    {
        public static int CalculateNumberOfTroopsToRemove(TroopRosterElement troop, List<LanceData> lances)
        {
            var character = troop.Character;
            int memberCount = troop.Number;
            int lanceCount = 0;
            for (int i = 0; i < lances.Count; i++)
                lanceCount += lances[i].LanceRoster.GetTroopCount(character);

            return Math.Max(0, lanceCount - memberCount);
        }
        public static void RemoveTroopsRandomlyFromLances(TroopRosterElement troop, int toRemove, List<LanceData> lances)
        {
            var character = troop.Character;
            if (toRemove <= 0 || lances.Count == 0)
                return;
            var counts = new int[lances.Count];
            int totalInLances = 0;
            for (int i = 0; i < lances.Count; i++)
            {
                counts[i] = lances[i].LanceRoster.GetTroopCount(character);
                totalInLances += counts[i];
            }
            toRemove = Math.Min(toRemove, totalInLances);

            var removeCounts = new int[lances.Count];
            int remainingToRemove = toRemove;

            var rnd = new Random();

            var indices = new List<int>();
            for (int i = 0; i < counts.Length; i++)
                if (counts[i] > 0)
                    indices.Add(i);

            while (remainingToRemove > 0 && indices.Count > 0)
            {
                int pick = rnd.Next(indices.Count);
                int i = indices[pick];

                int removeHere = Math.Min(remainingToRemove, counts[i] - removeCounts[i]);
                removeCounts[i] += removeHere;
                remainingToRemove -= removeHere;
                if (removeCounts[i] >= counts[i])
                    indices.RemoveAt(pick);
            }

            for (int i = 0; i < lances.Count; i++)
            {
                if (removeCounts[i] > 0)
                    lances[i].LanceRoster.AddToCounts(character, -removeCounts[i]);
            }
        }
    }

}
