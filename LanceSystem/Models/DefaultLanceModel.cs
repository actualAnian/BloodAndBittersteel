using LanceSystem.Deserialization;
using LanceSystem.LanceDataClasses;
using LanceSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LanceSystem.Models
{
    internal class DefaultLanceModel : LanceModel
    {
        static readonly Random random = new();
        public override int LancesFromClanTier(int clanTier)
        {
            return clanTier;
        }

        public override ExplainedNumber MaxLancesForParty(PartyBase party)
        {
            var number = new ExplainedNumber();
            number.Add(LancesFromClanTier(party.Owner.Clan.Tier), new("{lance_lances_from_clan}Lances from clan tier"));
            return number;
        }
        private static int BaseLanceCount => 20;

        public override ExplainedNumber GetMaxTroopsInLance(Hero notable)
        {
            var number = new ExplainedNumber();
            number.Add(BaseLanceCount, new("{=lance_lance_base}Base value"));
            GetTroopsInLanceFromRelation(notable, ref number);
            GetTroopsInLanceFromProsperity(notable, ref number);
            GetTroopsInLanceFromProjects(notable, ref number);
            return number;
        }
        public void GetTroopsInLanceFromRelation(Hero notable, ref ExplainedNumber value)
        {
            int num = (int)(notable.GetRelationWithPlayer() / 10);
            var text = new TextObject("{=lance_lance_size_from_relation}From relation");
            value.Add(num, text);
        }
        public void GetTroopsInLanceFromProsperity(Hero notable, ref ExplainedNumber value)
        {
            var text = new TextObject("{=lance_lance_size_from_prosperity}From prosperity");
            if (notable.BornSettlement.IsTown)
            {
                int num = (int)((notable.BornSettlement.Town.Prosperity - 2000f)/ 100);
                num = Math.Min(num, 20);
                value.Add(num, text);
            }
            else if (notable.BornSettlement.IsCastle)
            {
                int num = (int)((notable.BornSettlement.Town.Prosperity) / 100);
                num = Math.Min(num, 10);
                value.Add(num, text);
            }
            else
            {
                int num = (int)((notable.BornSettlement.Village.Hearth) / 100);
                num = Math.Min(num, 10);
                value.Add(num, text);
            }
        }
        public void GetTroopsInLanceFromProjects(Hero notable, ref ExplainedNumber value)
        {
            if (notable.BornSettlement.Town != null)
            {
                foreach(var building in notable.BornSettlement.Town.Buildings)
                {
                    var buildingType = building.BuildingType.StringId;
                    var level = building.CurrentLevel;
                    var gain = LanceModelUtils.GetTroopsFromBuildingTypeAndLevelFromItself(buildingType, level);
                    if (gain == 0) continue;
                    var explanationText = new TextObject("{=lance_from_project}From {PROJECT}, level {LEVEL}");
                    GameTexts.SetVariable("PROJECT", building.BuildingType.Name);
                    GameTexts.SetVariable("LEVEL", level);
                    value.Add(gain, explanationText);
                }
            }
            if (notable.BornSettlement.IsVillage)
            {
                foreach(var building in notable.BornSettlement.Village.Bound.Town.Buildings)
                {
                    var buildingType = building.BuildingType.StringId;
                    var level = building.CurrentLevel;
                    var gain = LanceModelUtils.GetTroopsFromBuildingTypeAndLevelFromBoundTown(buildingType, level);
                    if (gain == 0) continue;
                    var explanationText = new TextObject("{=lance_from_bound_project}From bound settlements {PROJECT}, level {LEVEL}");
                    GameTexts.SetVariable("PROJECT", building.BuildingType.Name);
                    GameTexts.SetVariable("LEVEL", level);
                    value.Add(gain, explanationText);
                }
            }
        }
        public override void UpdateNotablesLanceTroops(Hero notable, SettlementNotableLanceInfo lanceData)
        {
            RecruitNewNotableTroops(notable, lanceData);
            IncreaseNotableTroopsTier(notable, lanceData);
        }
        private void RecruitNewNotableTroops(Hero notable, SettlementNotableLanceInfo lanceData)
        {
            var availableLanceTroops = lanceData.CurrentNotableLanceTroopRoster;
            int troopsToGet = Math.Min(lanceData.CachedMaxLanceTroops.RoundedResultNumber, DailyTroopsGet(notable));
            LanceModelUtils.RecruitNTroopsToRoster(troopsToGet, availableLanceTroops, lanceData.CurrentLance.TroopsTemplate);
        }
        private void IncreaseNotableTroopsTier(Hero notable, SettlementNotableLanceInfo lanceData)
        {
            var troopsToUpgrade = random.Next(DailyTroopsToUpgrade(notable));
            while (troopsToUpgrade > 0)
            {
                var troopType = LanceModelUtils.ChooseNextTroopTypeToGet(lanceData.CurrentNotableLanceTroopRoster, lanceData.CurrentLance.TroopsTemplate);
                var troopToUpgrade = LanceModelUtils.GetNextTroopToUpgrade(lanceData.CachedMaxTroopPerTier, lanceData.CurrentNotableLanceTroopRoster, troopType);
                if (troopToUpgrade == null)
                    break;
                var possibleUpgrades = troopToUpgrade.UpgradeTargets.Where(t => LanceModelUtils.ClassFormationToLanceTroopType(t) == troopType);
                var goodUpgrades = possibleUpgrades.Where(c => LanceModelUtils.ClassFormationToLanceTroopType(c) == troopType);
                lanceData.CurrentNotableLanceTroopRoster.AddToCounts(goodUpgrades.GetRandomElementInefficiently(), 1);
                lanceData.CurrentNotableLanceTroopRoster.RemoveTroop(troopToUpgrade, 1);
            }
        }
        internal Dictionary<LanceTroopCategory, int> GetTroopTypeDistribution(TroopRoster roster)
        {
            var pairs = new List<(LanceTroopCategory type, int number)>();
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
            while (quality.Count < 4)
                quality.Add(0f);
            quality[2] += bonus * 0.1f;
            quality[3] += bonus * 0.05f;
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
            int number = 0;
            number += (int)notable.Power / 100;
            return number;
        }

        public override int DailyTroopsGet(Hero notable)
        {
            int number = 1;
            number += (int)notable.Power / 100;
            return number;
        }

        public int MaxMainPartySize(int tier)
        {
            return tier switch
            {
                0 => 10,
                1 => 10,
                2 => 15,
                3 => 20,
                4 => 25,
                5 => 30,
                _ => 30,
            };
        }
        public override ExplainedNumber GetRetinueSizeLimit(PartyBase party)
        {
            var number = new ExplainedNumber();
            number.Add(MaxMainPartySize(party.Owner.Clan.Tier), new("{=lance_lance_party_size_base}From clan tier"));
            GetRetinueSizeFromPerks(party, ref number);
            return number;
        }
        private void GetRetinueSizeFromPerks(PartyBase party, ref ExplainedNumber result)
        {
            var partyLeader = party.LeaderHero;
            var partyMapFaction = party.Owner.MapFaction;
            if (partyLeader == null) return;
            if (partyLeader.GetPerkValue(DefaultPerks.OneHanded.Prestige))
            {
                result.Add(DefaultPerks.OneHanded.Prestige.SecondaryBonus, DefaultPerks.OneHanded.Prestige.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.TwoHanded.Hope))
            {
                result.Add(DefaultPerks.TwoHanded.Hope.SecondaryBonus, DefaultPerks.TwoHanded.Hope.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Athletics.ImposingStature))
            {
                result.Add(DefaultPerks.Athletics.ImposingStature.SecondaryBonus, DefaultPerks.Athletics.ImposingStature.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Bow.MerryMen))
            {
                result.Add(DefaultPerks.Bow.MerryMen.PrimaryBonus, DefaultPerks.Bow.MerryMen.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Tactics.HordeLeader))
            {
                result.Add(DefaultPerks.Tactics.HordeLeader.PrimaryBonus, DefaultPerks.Tactics.HordeLeader.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Scouting.MountedScouts))
            {
                result.Add(DefaultPerks.Scouting.MountedScouts.SecondaryBonus, DefaultPerks.Scouting.MountedScouts.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Leadership.Authority))
            {
                result.Add(DefaultPerks.Leadership.Authority.SecondaryBonus, DefaultPerks.Leadership.Authority.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Leadership.UpliftingSpirit))
            {
                result.Add(DefaultPerks.Leadership.UpliftingSpirit.SecondaryBonus, DefaultPerks.Leadership.UpliftingSpirit.Name, null);
            }
            if (partyLeader.GetPerkValue(DefaultPerks.Leadership.TalentMagnet))
            {
                result.Add(DefaultPerks.Leadership.TalentMagnet.PrimaryBonus, DefaultPerks.Leadership.TalentMagnet.Name, null);
            }
            if (partyLeader.Clan.Leader == partyLeader)
            {
                if (partyLeader.Clan.Tier >= 5 && partyMapFaction.IsKingdomFaction && ((Kingdom)partyMapFaction).ActivePolicies.Contains(DefaultPolicies.NobleRetinues))
                {
                    result.Add(40f, DefaultPolicies.NobleRetinues.Name, null);
                }
                if (partyMapFaction.IsKingdomFaction && partyMapFaction.Leader == partyLeader && ((Kingdom)partyMapFaction).ActivePolicies.Contains(DefaultPolicies.RoyalGuard))
                {
                    result.Add(60f, DefaultPolicies.RoyalGuard.Name, null);
                }
            }
        }
    }
}