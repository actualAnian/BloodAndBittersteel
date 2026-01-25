using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using System;
using System.Linq;
using LanceSystem.Deserialization;

namespace LanceSystem
{

    internal static class LanceModelUtils
    {
        static readonly Random random = new();
        public static LanceTroopCategory ChooseNextTroopTypeToGet(TroopRoster roster, LanceTroopsTemplate lanceTemplate)
        {
            var troops = GetTroopTypeDistribution(roster);
            return DetermineTroopTypeToAdd(troops, lanceTemplate);
        }
        internal static LanceTroopCategory DetermineTroopTypeToAdd(Dictionary<LanceTroopCategory, int>? currentTroops, LanceTroopsTemplate lanceTemplate)
        {
            // Extract likelihoods (expected to be normalized between 0 and 1)
            var likelihoods = lanceTemplate.TroopTypes
            .GroupBy(t => t.Category)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Likelihood)
            );

            var counts = new Dictionary<LanceTroopCategory, int>
            {
                { LanceTroopCategory.Infantry, 0 },
                { LanceTroopCategory.Ranged, 0 },
                { LanceTroopCategory.Cavalry, 0 },
                { LanceTroopCategory.HorseArcher, 0 }
            };

            if (currentTroops != null)
            {
                foreach (var kv in currentTroops)
                    counts[kv.Key] = kv.Value;
            }

            int total = 0;
            foreach (var v in counts.Values) total += v;

            // Find candidates where adding one does NOT exceed the likelihood target
            var acceptable = new List<(LanceTroopCategory type, double gap)>();
            foreach (var kv in counts)
            {
                var type = kv.Key;
                var cnt = kv.Value;
                double newShare = (total + 1) > 0 ? (double)(cnt + 1) / (double)(total + 1) : 1.0;
                double target = likelihoods[type];
                if (newShare <= target + 1e-12)
                {
                    acceptable.Add((type, target - newShare));
                }
            }

            if (acceptable.Count > 0)
            {
                acceptable.Sort((a, b) => b.gap.CompareTo(a.gap));
                return acceptable[0].type;
            }

            // If none acceptable, pick the troop type with the highest likelihood
            var bestType = LanceTroopCategory.Infantry;
            double bestLikelihood = -1.0;
            foreach (var kv in likelihoods)
            {
                if (kv.Value > bestLikelihood)
                {
                    bestLikelihood = kv.Value;
                    bestType = kv.Key;
                }
            }

            return bestType;
        }
        public static string ChooseNextTroopToRecruit(
            LanceTroopsTemplate template,
            LanceTroopCategory troopType)
        {
            var candidates = template.TroopTypes
                .Where(t => t.Category == troopType)
                .ToList();

            if (candidates.Count == 0)
                throw new InvalidOperationException($"No troops defined for {troopType}");

            if (candidates.Count == 1)
                return candidates[0].BasicTroopId;

            var total = candidates.Sum(t => t.Likelihood);
            var roll = random.NextDouble() * total;

            double acc = 0;
            foreach (var troop in candidates)
            {
                acc += troop.Likelihood;
                if (roll <= acc)
                    return troop.BasicTroopId;
            }
            return candidates[0].BasicTroopId;
        }

        internal static Dictionary<LanceTroopCategory, int> GetTroopTypeDistributionFromPairs(IEnumerable<(LanceTroopCategory type, int number)>? pairs)
        {
            var res = new Dictionary<LanceTroopCategory, int>
            {
                { LanceTroopCategory.Infantry, 0 },
                { LanceTroopCategory.Ranged, 0 },
                { LanceTroopCategory.Cavalry, 0 },
                { LanceTroopCategory.HorseArcher, 0 }
            };

            if (pairs == null) return res;

            foreach (var p in pairs)
            {
                if (!res.ContainsKey(p.type)) res[p.type] = 0;
                res[p.type] += p.number;
            }

            return res;
        }
        internal static Dictionary<LanceTroopCategory, int> GetTroopTypeDistribution(TroopRoster roster)
        {
            var pairs = new List<(LanceTroopCategory type, int number)>();
            foreach (var troop in roster.GetTroopRoster())
            {
                var type = ClassFormationToLanceTroopType(troop.Character.DefaultFormationClass);
                var number = troop.Number;
                pairs.Add((type, number));
            }
            return GetTroopTypeDistributionFromPairs(pairs);
        }

        private static LanceTroopCategory ClassFormationToLanceTroopType(FormationClass troopType)
        {
            return troopType switch
            {
                FormationClass.Ranged or FormationClass.Skirmisher => LanceTroopCategory.Ranged,
                FormationClass.Cavalry or FormationClass.HeavyCavalry or FormationClass.LightCavalry => LanceTroopCategory.Cavalry,
                FormationClass.HorseArcher => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry,
            };
        }
        public static int GetTroopsFromBuildingTypeAndLevelFromBoundTown(string buildingType, int level)
        {
            return buildingType switch
            {
                "building_settlement_roads_and_paths" => level switch
                {
                    1 => 1,
                    2 => 3,
                    3 => 5,
                    _ => 1,
                },
                "building_castle_roads_and_paths" => level switch
                {
                    1 => 1,
                    2 => 3,
                    3 => 5,
                    _ => 1,
                },
                _ => 0,
            };
        }

        public static int GetTroopsFromBuildingTypeAndLevelFromItself(string buildingType, int level)
        {
            return buildingType switch
            {
                "building_settlement_barracks" => level switch
                {
                    1 => 2,
                    2 => 4,
                    3 => 5,
                    _ => 2,
                },
                "building_castle_barracks" => level switch
                {
                    1 => 2,
                    2 => 4,
                    3 => 5,
                    _ => 2,
                },
                _ => 0,
            };
        }

        internal static List<float>? GetTroopQualityFromBuildingTypeAndLevel(string buildingType, int level)
        {
            return buildingType switch
            {
                "building_settlement_training_fields" or "building_castle_training_fields" => level switch
                {
                    1 => new List<float>() { 0, 0, 0, 0.3f },
                    2 => new List<float>() { 0, 0, 0, 0.3f, 0.2f, 0.1f },
                    3 => new List<float>() { 0, 0, 0, 0.3f, 0.3f, 0.2f },
                    _ => new List<float>() { 0, 0, 0, 0.3f },
                },
                "building_settlement_guard_house" or "building_castle_guard_house" => level switch
                {
                    1 => new List<float>() { 0, 0, 0.2f, 0.3f },
                    2 => new List<float>() { 0, 0, 0.3f, 0.5f },
                    3 => new List<float>() { 0, 0, 0.5f, 0.5f, 0.2f },
                    _ => new List<float>() { 0, 0, 0.2f, 0.3f },
                },
                _ => null,
            };
        }
        public static void ClampTroopQuality(List<float> troopQuality)
        {
            float sum = 0;
            for (int i = troopQuality.Count - 1; i >= 0; i--)
            {
                sum += troopQuality[i];
                if (sum > 1.0f)
                {
                    troopQuality[i] -= (sum - 1.0f);
                    sum = 1.0f;
                }
            }
        }
        public static LanceTroopCategory ClassFormationToLanceTroopType(CharacterObject troop)
        {
            var troopType = troop.DefaultFormationClass;
            return troopType switch
            {
                FormationClass.Ranged or FormationClass.Skirmisher => LanceTroopCategory.Ranged,
                FormationClass.Cavalry or FormationClass.HeavyCavalry or FormationClass.LightCavalry => LanceTroopCategory.Cavalry,
                FormationClass.HorseArcher => LanceTroopCategory.HorseArcher,
                _ => LanceTroopCategory.Infantry,
            };
        }
        // @TODO add tests for this method
        internal static CharacterObject? GetNextTroopToUpgrade(List<float> cachedMaxTroopPerTier, TroopRoster currentNotableLanceTroopRoster, LanceTroopCategory troopType)
        {
            var elements = currentNotableLanceTroopRoster.GetTroopRoster();
            int total = 0;
            var countsPerTier = new Dictionary<int, int>();

            foreach (var el in elements)
            {
                total += el.Number;
                int tier = el.Character.Tier;
                if (!countsPerTier.ContainsKey(tier)) countsPerTier[tier] = 0;
                countsPerTier[tier] += el.Number;
            }

            var candidates = new List<(CharacterObject obj, int weight)>();

            foreach (var el in elements)
            {
                if (el.Number <= 0) continue;
                var src = el.Character;

                var upgradeTargets = src.UpgradeTargets;
                if (upgradeTargets == null) continue;

                bool hasValidTarget = false;
                foreach (var up in upgradeTargets)
                {
                    var form = up.DefaultFormationClass;
                    if (LanceModelUtils.ClassFormationToLanceTroopType(up) != troopType) continue;

                    int targetTier = up.Tier;
                    int countAtTarget = countsPerTier.ContainsKey(targetTier) ? countsPerTier[targetTier] : 0;
                    double share = total > 0 ? (double)countAtTarget / (double)total : 0.0;

                    // If cached list contains a cap for this tier, enforce it as a proportion (clamp when >= cap)
                    if (targetTier >= 0 && targetTier < cachedMaxTroopPerTier.Count)
                    {
                        var cap = cachedMaxTroopPerTier[targetTier];
                        if (share >= cap - 1e-9)
                        {
                            // cannot upgrade to this target tier because it is already at or above cap
                            continue;
                        }
                    }

                    hasValidTarget = true;
                    break;
                }

                if (hasValidTarget)
                {
                    candidates.Add((src, el.Number));
                }
            }

            if (candidates.Count == 0) return null;

            // Weighted random selection by existing counts
            int weightSum = candidates.Sum(c => c.weight);
            if (weightSum <= 0) return candidates[0].obj;
            int r = random.Next(weightSum);
            int acc = 0;
            foreach (var c in candidates)
            {
                acc += c.weight;
                if (r < acc) return c.obj;
            }
            return candidates[0].obj;
        }
    }
}