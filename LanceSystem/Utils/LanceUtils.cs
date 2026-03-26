using LanceSystem.LanceDataClasses;
using LanceSystem.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace LanceSystem.Utils
{
    public static class LanceUtils
    {
        public static Random UtilsRandom = new();
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

            foreach (var tier in tiers)
            {
                var sameTier = entries.Where(e => e.Tier == tier && e.Count > 0).ToList();
                for (int i = sameTier.Count - 1; i > 0; i--)
                {
                    int j = UtilsRandom.Next(i + 1);
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
        //public static int CalculateNumberOfTroopsToRemove(TroopRosterElement troop, TroopRoster roster)
        //{
        //    var character = troop.Character;
        //    int memberCount = troop.Number;
        //    int lanceCount = roster.GetTroopCount(character);

        //    return Math.Max(0, lanceCount - memberCount);
        //}
        public static void UpgradeTroopsRandomlyInLances(CharacterObject from, CharacterObject to, int toAdd, List<LanceData> lances)
        {
            if (toAdd <= 0 || lances.Count == 0)
                return;

            var counts = new int[lances.Count];
            int totalAvailable = 0;

            for (int i = 0; i < lances.Count; i++)
            {
                counts[i] = lances[i].LanceRoster.GetTroopCount(from);
                totalAvailable += counts[i];
            }
            if (toAdd > totalAvailable)
            {
                //LanceLogger.Logger.Warning(
                //    $"UpgradeTroopsRandomlyInLances requested {toAdd} upgrades but only {totalAvailable} '{from?.StringId}' troops available.");
            }
            toAdd = Math.Min(toAdd, totalAvailable);
            if (toAdd == 0) return;
            for (int i = 0; i < lances.Count; i++)
            {
                counts[i] = lances[i].LanceRoster.GetTroopCount(from);
                totalAvailable += counts[i];
            }

            toAdd = Math.Min(toAdd, totalAvailable);

            var upgradeCounts = new int[lances.Count];
            int remaining = toAdd;

            var indices = new List<int>();
            for (int i = 0; i < counts.Length; i++)
                if (counts[i] > 0)
                    indices.Add(i);

            while (remaining > 0 && indices.Count > 0)
            {
                int pick = UtilsRandom.Next(indices.Count);
                int i = indices[pick];

                upgradeCounts[i]++;
                remaining--;

                if (upgradeCounts[i] >= counts[i])
                    indices.RemoveAt(pick);
            }

            for (int i = 0; i < lances.Count; i++)
            {
                int amount = upgradeCounts[i];
                if (amount > 0)
                {
                    lances[i].LanceRoster.AddToCounts(from, -amount);
                    lances[i].LanceRoster.AddToCounts(to, amount);
                }
            }
        }

        public static void NormalizeLanceTroopsToParty(TroopRoster partyRoster, List<LanceData> lances)
        {
            var troopCounts = new Dictionary<CharacterObject, (int party, int lances)>();

            foreach (var troop in partyRoster.GetTroopRoster())
                troopCounts[troop.Character] = (troop.Number, 0);

            foreach (var lance in lances)
            {
                foreach (var troop in lance.LanceRoster.GetTroopRoster())
                {
                    if (troopCounts.TryGetValue(troop.Character, out var counts))
                        troopCounts[troop.Character] = (counts.party, counts.lances + troop.Number);
                    else
                        troopCounts[troop.Character] = (0, troop.Number);
                }
            }
            foreach (var kvp in troopCounts)
            {
                var character = kvp.Key;
                int partyCount = kvp.Value.party;
                int lanceCount = kvp.Value.lances;
                int excess = lanceCount - partyCount;
                if (excess <= 0)
                    continue;
                RemoveTroopsRandomlyFromLances(character, excess, lances);
            }
        }

        public static void RemoveTroopsRandomlyFromLances(CharacterObject character, int toRemove, List<LanceData> lances)
        {
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

            var indices = new List<int>();
            for (int i = 0; i < counts.Length; i++)
                if (counts[i] > 0)
                    indices.Add(i);

            while (remainingToRemove > 0 && indices.Count > 0)
            {
                int pick = UtilsRandom.Next(indices.Count);
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
        public static void RemoveLowestTierTroops(TroopRoster roster, int amount)
        {
            if (roster == null || amount <= 0)
                return;
            var troops = roster.GetTroopRoster()
                .Where(t => t.Number > 0)
                .Select(t => new
                {
                    t.Character,
                    Count = t.Number,
                    Tier = t.Character?.Tier ?? int.MaxValue
                })
                .OrderBy(t => t.Tier)
                .ToList();
            int remaining = amount;

            foreach (var troop in troops)
            {
                if (remaining <= 0)
                    break;
                int remove = Math.Min(remaining, troop.Count);
                roster.AddToCounts(troop.Character, -remove);
                remaining -= remove;
            }
        }
    }
}