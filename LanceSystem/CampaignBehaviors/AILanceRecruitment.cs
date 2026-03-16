using Helpers;
using LanceSystem.LanceDataClasses;
using LanceSystem.Models;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.CampaignBehaviors
{
    // removing RecruitmentCampaignBehavior.UpdateVolunteersOfNotablesInSettlement should prevent the ai from recruiting any units

    internal class AILanceRecruitment : CampaignBehaviorBase
    {
        const float MaxScoreFromDistance = 1f;
        const float FreeLancesMultiplier = 1f;
        const float FilledLanceMultiplier = 1f;
        public const float FilledLancePercentageToConsiderTaking = 0.5f;

        const float RefillLanceMultiplier = 2f;
        const float LanceStrengthToConsiderRefilling = 0.6f;
        const float LanceStrengthToConsiderDisbanding = 0.3f;

        LancesCampaignBehavior? _behavior;
        LancesCampaignBehavior LancesBehavior
        {
            get
            {
                _behavior ??= Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
                return _behavior;
            }
        }
        LanceModel? _model;
        LanceModel LanceModel 
        {
            get 
            {
                _model ??= Campaign.Current.Models.LanceModel();
                return _model;
            }
        }
        public override void RegisterEvents()
        {
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, OnSettlementEntered);
            CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, AiLanceTick);
        }
        private void AiLanceTick(MobileParty party, PartyThinkParams p)
        {
            ConsiderRecruitingLances(party, p);
            ConsiderRefillingLances(party, p);
        }
        private void ConsiderRecruitingLances(MobileParty party, PartyThinkParams p)
        {
            if (party.StringId == "lord_A9_l_party_1")
            { int a = 5; }
            if (!LanceModel.IsUsingLanceSystem(party.Party)) return;
            if (LanceModel.MaxLancesForParty(party.Party).RoundedResultNumber <= party.Party.Lances().Count
                && !HasLancesToBeDisbanded(party)) return;
            var ownedSettlements = party.Owner.Clan.Settlements;
            foreach (var settlement in ownedSettlements)
            {
                if (settlement.SiegeEvent != null) continue;
                if (!settlement.IsTown && !settlement.IsCastle) continue;
                if (!CanAffordLance(party, settlement)) return;
                var score = ScoreFromDistance(party, settlement) + 
                    ScoreFromFreeLanceSpots(party) + 
                    ScoreFromNotableLanceStrength(settlement);
                AddBehaviorTupleWithScore(p, settlement, score);
            }
        }
        private bool HasLancesToBeDisbanded(MobileParty party)
        {
            return party.Party.Lances().Any(l => ((float)l.TotalManCount / l.MaxSize) < LanceStrengthToConsiderDisbanding);
        }
        private void ConsiderRefillingLances(MobileParty party, PartyThinkParams p)
        {
            foreach (LanceData lance in party.Party.Lances())
            {
                if (lance is not NotableLanceData notableLance) continue;
                var lanceInfo = notableLance.GetSettlementNotableLanceInfo();
                var settlement = MBObjectManager.Instance.GetObject<CharacterObject>(lanceInfo.NotableId).HeroObject.CurrentSettlement;
                var maxLanceMembers = lanceInfo.CachedMaxLanceTroops;
                var missingPercentage = (maxLanceMembers.ResultNumber - lance.MaxSize) / maxLanceMembers.ResultNumber;
                if (1-missingPercentage < LanceStrengthToConsiderRefilling) continue;
                var score = missingPercentage * RefillLanceMultiplier + ScoreFromDistance(party, settlement);
                AddBehaviorTupleWithScore(p, settlement, score);
            }
        }
        private void AddBehaviorTupleWithScore(PartyThinkParams p, Settlement settlement, float visitingNearbySettlementScore)
        {
            var data = new AIBehaviorData(settlement, AiBehavior.GoToSettlement, MobileParty.NavigationType.All, false, false, false);
            if (p.TryGetBehaviorScore(data, out float num))
            {
                p.SetBehaviorScore(data, num + visitingNearbySettlementScore);
                return;
            }
            p.AddBehaviorScore(new ValueTuple<AIBehaviorData, float>(data, visitingNearbySettlementScore));
        }

        private float ScoreFromDistance(MobileParty party, Settlement settlement)
        {
            float maxDistance = Campaign.MapDiagonal / 2;
            AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(party, settlement, settlement.HasPort, out var navigationType, out var distance, out var fromPort);
            distance = Math.Min(distance, maxDistance);
            distance /= maxDistance * MaxScoreFromDistance;
            return MaxScoreFromDistance - distance;
        }
        private float ScoreFromFreeLanceSpots(MobileParty party)
        {
            return (LanceModel.MaxLancesForParty(party.Party).RoundedResultNumber - party.Party.Lances().Count) * FreeLancesMultiplier; 
        }
        private float ScoreFromNotableLanceStrength(Settlement settlement)
        {
            var score = LancesBehavior.GetMostReadyLance(settlement) * FilledLanceMultiplier;
            return score;
        }
        private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
        {
            //var aa = MobileParty.All.Where(p => p.StringId == "lord_A9_l_party_1").FirstOrDefault();
            //var test = aa.ThinkParamsCache;
            if (party  == null || party == MobileParty.MainParty) return;
            if (!LanceModel.IsUsingLanceSystem(party.Party)) return;
            TryRefillLances(party, settlement);
            while (CanRecruitLance(party, settlement))
            {
                DisbandLanceIfNecessary(party);
                RecruitLance(party, settlement);
            }
        }

        private void DisbandLanceIfNecessary(MobileParty party)
        {
            if (!party.Party.HasFreeLanceSlots()) return;
            foreach (var lance in party.Party.Lances())
                if (lance.TotalManCount / lance.MaxSize < LanceStrengthToConsiderDisbanding)
                {
                    LancesBehavior.DisbandLanceInParty(party.Party, lance, true);
                    return;
                }
        }

        private void TryRefillLances(MobileParty party, Settlement settlement)
        {
            foreach(var lance in party.Party.Lances())
            {
                if (lance is not NotableLanceData notableLance) continue;
                var notable = MBObjectManager.Instance.GetObject<CharacterObject>(notableLance.NotableId);
                if (notable.HeroObject.CurrentSettlement != settlement) continue;
                LancesBehavior.RefillLanceTroops(notableLance, party.Party);
            }
        }
        private bool CanRecruitLance(MobileParty party, Settlement settlement)
        {
            if (party.LeaderHero == null || party.ActualClan.Leader != settlement.Owner) return false;
            if (!LancesBehavior.DoesSettlementHaveFreeLances(settlement)) return false;
            if (LanceModel.MaxLancesForParty(party.Party).RoundedResultNumber <= party.Party.Lances().Count && !HasLancesToBeDisbanded(party)) return false;
            return true;
        }
        private bool CanAffordLance(MobileParty party, Settlement settlement)
        {
            var hero = LancesBehavior.GetHeroWithStrongestLanceInSettlement(settlement);
            if (hero == null) return false;
            var roster = LancesBehavior.GetNotableData(hero.StringId).CurrentNotableLanceTroopRoster;
            return party.Owner.Gold > TroopRosterExtensions.CalculateTroopRosterDailyCost(roster) * 7;
        }
        private void RecruitLance(MobileParty party, Settlement settlement)
        {
            var hero = LancesBehavior.GetHeroWithStrongestLanceInSettlement(settlement);
            if (hero == null) return;
            LancesBehavior.RecruitNotableLanceToParty(party.Party, hero);
        }
        public override void SyncData(IDataStore dataStore) { }
    }
}
