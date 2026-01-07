using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.LanceSystem
{
    internal class BaBLanceModel : LanceModel
    {
        static Random random = new();
        public override int LancesFromClanTier(int clanTier)
        {
            return clanTier;
        }

        public override ExplainedNumber MaxLancesForParty(PartyBase party)
        {
            var number = new ExplainedNumber();
            number.Add(LancesFromClanTier(party.Owner.Clan.Tier), new("{bab_lances_from_clan}Lances from clan tier"));
            return number;
        }
        private static int BaseLanceCount => 20;

        public override ExplainedNumber GetMaxTroopsInLance(Hero notable)
        {
            var number = new ExplainedNumber();
            number.Add(BaseLanceCount, new("{=bab_lance_base}Base value"));
            GetTroopsInLanceFromRelation(notable, number);
            GetTroopsInLanceFromProsperity(notable, number);
            GetTroopsInLanceFromProjects(notable, number);
            return number;
        }
        public void GetTroopsInLanceFromRelation(Hero notable, ExplainedNumber value)
        {
            int num = (int)(notable.GetRelationWithPlayer() / 10);
            var text = new TextObject("{=bab_lance_size_from_relation}From relation");
            value.Add(num, text);
        }
        public void GetTroopsInLanceFromProsperity(Hero notable, ExplainedNumber value)
        {
            var text = new TextObject("{=bab_lance_size_from_prosperity}From prosperity");
            if (notable.CurrentSettlement.IsTown)
            {
                int num = (int)((notable.CurrentSettlement.Town.Prosperity - 2000f)/ 100);
                num = Math.Min(num, 20);
                value.Add(num, text);
            }
            else if (notable.CurrentSettlement.IsCastle)
            {
                int num = (int)((notable.CurrentSettlement.Town.Prosperity) / 100);
                num = Math.Min(num, 10);
                value.Add(num, text);
            }
            else
            {
                int num = (int)((notable.CurrentSettlement.Village.Hearth) / 100);
                num = Math.Min(num, 10);
                value.Add(num, text);
            }
        }
        public void GetTroopsInLanceFromProjects(Hero notable, ExplainedNumber value)
        {
            if (notable.CurrentSettlement.Town != null)
            {
                foreach(var building in notable.CurrentSettlement.Town.Buildings)
                {
                    var buildingType = building.BuildingType.StringId;
                    var level = building.CurrentLevel;
                    var gain = LanceModelUtils.GetTroopsFromBuildingTypeAndLevelFromItself(buildingType, level);
                    if (gain == 0) continue;
                    var explanationText = new TextObject("{=bab_from_project}From {PROJECT}, level {LEVEL}");
                    GameTexts.SetVariable("PROJECT", building.BuildingType.Name);
                    GameTexts.SetVariable("LEVEL", level);
                    value.Add(gain, explanationText);
                }
            }
            if (notable.CurrentSettlement.IsVillage)
            {
                foreach(var building in notable.CurrentSettlement.Village.Bound.Town.Buildings)
                {
                    var buildingType = building.BuildingType.StringId;
                    var level = building.CurrentLevel;
                    var gain = LanceModelUtils.GetTroopsFromBuildingTypeAndLevelFromBoundTown(buildingType, level);
                    if (gain == 0) continue;
                    var explanationText = new TextObject("{=bab_from_bound_project}From bound settlements {PROJECT}, level {LEVEL}");
                    GameTexts.SetVariable("PROJECT", building.BuildingType.Name);
                    GameTexts.SetVariable("LEVEL", level);
                    value.Add(gain, explanationText);
                }
            }
        }
        public override void UpdateNotablesLanceTroops(Hero notable, NotableLanceData lanceData)
        {
            RecruitNewNotableTroops(notable, lanceData);
            IncreaseNotableTroopsTier(notable, lanceData);
        }
        private void RecruitNewNotableTroops(Hero notable, NotableLanceData lanceData)
        {
            var availableLanceTroops = lanceData.CurrentNotableLanceTroopRoster;
            if (availableLanceTroops.Count < lanceData.CachedMaxLanceTroops.RoundedResultNumber)
            {
                availableLanceTroops.AddToCounts(notable.Culture.EliteBasicTroop, 1);
            }
        }
        private void IncreaseNotableTroopsTier(Hero notable, NotableLanceData lanceData)
        {
            var troopsToUpgrade = random.Next(DailyTroopsToUpgrade(notable));
            while (troopsToUpgrade > 0)
            {
                var troopType = LanceModelUtils.ChooseNextTroopTypeToGet(lanceData.CurrentNotableLanceTroopRoster, lanceData.CurrentTroopTemplate);
                var troopToUpgrade = LanceModelUtils.GetNextTroopToUpgrade(lanceData.CachedMaxTroopPerTier, lanceData.CurrentNotableLanceTroopRoster, troopType);
                if (troopToUpgrade == null)
                    break;
                var possibleUpgrades = troopToUpgrade.UpgradeTargets.Where(t => LanceModelUtils.ClassFormationToLanceTroopType(t) == troopType);
                lanceData.CurrentNotableLanceTroopRoster.AddToCounts(possibleUpgrades.GetRandomElementInefficiently(), 1);
                lanceData.CurrentNotableLanceTroopRoster.RemoveTroop(troopToUpgrade, 1);
            }

        }
        internal Dictionary<LanceTroopType, int> GetTroopTypeDistribution(TroopRoster roster)
        {
            var pairs = new List<(LanceTroopType type, int number)>();
            foreach (var troop in roster.GetTroopRoster())
            {
                var type = LanceModelUtils.ClassFormationToLanceTroopType(troop.Character);
                var number = troop.Number;
                pairs.Add((type, number));
            }
            return LanceModelUtils.GetTroopTypeDistributionFromPairs(pairs);
        }
        readonly List<float> _defaultTroopQuality = new() { 0.0f, 0.7f, 0.3f};
        public override List<float> DefaultTroopQuality => _defaultTroopQuality;
        public override List<float> GetLanceTroopQuality(Hero notable)
        {
            var troopQuality = new List<float>(DefaultTroopQuality);
            GetTroopQualityFromProjects(notable, troopQuality);
            GetTroopQualityFromNotableInfluence(notable, troopQuality);
            LanceModelUtils.ClampTroopQuality(troopQuality);
            return troopQuality;
        }
        private void GetTroopQualityFromNotableInfluence(Hero notable, List<float> quality)
        {
            var bonus = notable.Power / 100;
            while (quality.Count < 3)
                quality.Add(0f);
            quality[2] += bonus * 0.6f;
            quality[3] += bonus * 0.4f;
        }
        private void GetTroopQualityFromProjects(Hero notable, List<float> quality)
        {
            if (notable.CurrentSettlement.Town != null)
            {
                foreach (var building in notable.CurrentSettlement.Town.Buildings)
                {
                    var buildingType = building.BuildingType.StringId;
                    var level = building.CurrentLevel;
                    var gain = LanceModelUtils.GetTroopQualityFromBuildingTypeAndLevel(buildingType, level);
                    if (gain == null) continue;
                    for (int i = 0; i < quality.Count; i++)
                    {
                        if (quality.Count <= i)
                            quality.Add(0);
                        quality[i] += gain[i];
                    }
                }
            }
        }

        public override int DailyTroopsToUpgrade(Hero notable)
        {
            int number = 2;
            number += (int)notable.Power % 50;
            return number;
        }
    }
}