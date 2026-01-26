using LanceSystem.LanceDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;

namespace LanceSystem.Utils
{
    public static class LanceUtils
    {
        public static void TransferTroopsBetweenTroopRosters(TroopRoster fromRoster, TroopRoster toRoster, int amount, int maxAmountInToRoster)
        {
            if (amount <= 0) return;

            // Compute current total in toRoster
            var toElements = toRoster.GetTroopRoster();
            int currentToTotal = toElements.Sum(e => e.Number);

            int remainingCapacity = maxAmountInToRoster - currentToTotal;
            if (remainingCapacity <= 0) return;

            int remainingToMove = Math.Min(amount, remainingCapacity);
            if (remainingToMove <= 0) return;

            // Build entries from fromRoster
            var fromElements = fromRoster.GetTroopRoster();
            var entries = new List<(CharacterObject Character, int Count, int Tier)>();
            foreach (var el in fromElements)
            {
                if (el.Number <= 0) continue;
                var ch = el.Character;
                if (ch == null) continue;
                entries.Add((ch, el.Number, ch.Tier));
            }

            if (entries.Count == 0) return;

            var tiers = entries.Select(e => e.Tier).Distinct().OrderByDescending(t => t).ToList();
            var rnd = new Random();

            foreach (var tier in tiers)
            {
                var sameTier = entries.Where(e => e.Tier == tier && e.Count > 0).ToList();
                for (int i = sameTier.Count - 1; i > 0; i--)
                {
                    int j = rnd.Next(i + 1);
                    (sameTier[j], sameTier[i]) = (sameTier[i], sameTier[j]);
                }

                foreach (var (Character, Count, Tier) in sameTier)
                {
                    if (remainingToMove <= 0) break;
                    int available = fromRoster.GetTroopCount(Character);
                    if (available <= 0) continue;

                    int move = Math.Min(available, remainingToMove);
                    if (move <= 0) continue;

                    fromRoster.AddToCounts(Character, -move);
                    toRoster.AddToCounts(Character, move);

                    remainingToMove -= move;
                    for (int k = 0; k < entries.Count; k++)
                    {
                        if (ReferenceEquals(entries[k].Character, Character))
                        {
                            entries[k] = (entries[k].Character, Math.Max(0, entries[k].Count - move), entries[k].Tier);
                            break;
                        }
                    }
                }

                if (remainingToMove <= 0) break;
            }
        }
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
