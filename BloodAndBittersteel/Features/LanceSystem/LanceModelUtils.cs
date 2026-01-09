using BloodAndBittersteel.Features.LanceSystem.Deserialization;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem;
using System;
using System.Linq;

namespace BloodAndBittersteel.Features.LanceSystem
{
    internal enum LanceTroopType
    {
        Infantry,
        Ranged,
        Cavalry,
        HorseArcher
    };

    internal static class LanceModelUtils
    {
        public static LanceTroopType ChooseNextTroopTypeToGet(TroopRoster roster, LanceTroopsTemplate lanceTemplate)
        {
            var troops = GetTroopTypeDistribution(roster);
            return DetermineTroopTypeToAdd(troops, lanceTemplate);
        }
        internal static LanceTroopType DetermineTroopTypeToAdd(Dictionary<LanceTroopType, int>? currentTroops, LanceTroopsTemplate lanceTemplate)
        {
            // Extract likelihoods (expected to be normalized between 0 and 1)
            var likelihoods = new Dictionary<LanceTroopType, double>
            {
                { LanceTroopType.Infantry, Safe(lanceTemplate?.MeleeTroop) },
                { LanceTroopType.Ranged, Safe(lanceTemplate?.RangedTroop) },
                { LanceTroopType.Cavalry, Safe(lanceTemplate?.CavalryTroop) },
                { LanceTroopType.HorseArcher, Safe(lanceTemplate?.HorseArcherTroop) }
            };

            static double Safe(TroopData? d) => d?.Likelihood ?? 0.0;

            // Ensure currentTroops has all keys
            var counts = new Dictionary<LanceTroopType, int>
            {
                { LanceTroopType.Infantry, 0 },
                { LanceTroopType.Ranged, 0 },
                { LanceTroopType.Cavalry, 0 },
                { LanceTroopType.HorseArcher, 0 }
            };

            if (currentTroops != null)
            {
                foreach (var kv in currentTroops)
                    counts[kv.Key] = kv.Value;
            }

            int total = 0;
            foreach (var v in counts.Values) total += v;

            // Find candidates where adding one does NOT exceed the likelihood target
            var acceptable = new List<(LanceTroopType type, double gap)>();
            foreach (var kv in counts)
            {
                var type = kv.Key;
                var cnt = kv.Value;
                double newShare = (total + 1) > 0 ? (double)(cnt + 1) / (double)(total + 1) : 1.0;
                double target = likelihoods[type];
                if (newShare <= target + 1e-12) // allow tiny epsilon
                {
                    acceptable.Add((type, target - newShare));
                }
            }

            if (acceptable.Count > 0)
            {
                // Choose the type with the largest gap (most underrepresented)
                acceptable.Sort((a, b) => b.gap.CompareTo(a.gap));
                return acceptable[0].type;
            }

            // If none acceptable, pick the troop type with the highest likelihood
            LanceTroopType bestType = LanceTroopType.Infantry;
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

        internal static Dictionary<LanceTroopType, int> GetTroopTypeDistributionFromPairs(IEnumerable<(LanceTroopType type, int number)>? pairs)
        {
            var res = new Dictionary<LanceTroopType, int>
            {
                { LanceTroopType.Infantry, 0 },
                { LanceTroopType.Ranged, 0 },
                { LanceTroopType.Cavalry, 0 },
                { LanceTroopType.HorseArcher, 0 }
            };

            if (pairs == null) return res;

            foreach (var p in pairs)
            {
                if (!res.ContainsKey(p.type)) res[p.type] = 0;
                res[p.type] += p.number;
            }

            return res;
        }
        internal static Dictionary<LanceTroopType, int> GetTroopTypeDistribution(TroopRoster roster)
        {
            var pairs = new List<(LanceTroopType type, int number)>();
            foreach (var troop in roster.GetTroopRoster())
            {
                var type = ClassFormationToLanceTroopType(troop.Character.DefaultFormationClass);
                var number = troop.Number;
                pairs.Add((type, number));
            }
            return GetTroopTypeDistributionFromPairs(pairs);
        }

        private static LanceTroopType ClassFormationToLanceTroopType(FormationClass troopType)
        {
            return troopType switch
            {
                FormationClass.Ranged or FormationClass.Skirmisher => LanceTroopType.Ranged,
                FormationClass.Cavalry or FormationClass.HeavyCavalry or FormationClass.LightCavalry => LanceTroopType.Cavalry,
                FormationClass.HorseArcher => LanceTroopType.HorseArcher,
                _ => LanceTroopType.Infantry,
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
        public static LanceTroopType ClassFormationToLanceTroopType(CharacterObject troop)
        {
            var troopType = troop.DefaultFormationClass;
            return troopType switch
            {
                FormationClass.Ranged or FormationClass.Skirmisher => LanceTroopType.Ranged,
                FormationClass.Cavalry or FormationClass.HeavyCavalry or FormationClass.LightCavalry => LanceTroopType.Cavalry,
                FormationClass.HorseArcher => LanceTroopType.HorseArcher,
                _ => LanceTroopType.Infantry,
            };
        }
        static Random random = new();
        // @TODO add tests for this method
        internal static CharacterObject? GetNextTroopToUpgrade(List<float> cachedMaxTroopPerTier, TroopRoster currentNotableLanceTroopRoster, LanceTroopType troopType)
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