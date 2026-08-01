using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.Jousting
{
    internal class AiJoustingBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, OnAiHourlyTick);
        }
        private void OnAiHourlyTick(MobileParty party, PartyThinkParams p)
        {
            foreach(var tournamentTown in GetAllActiveTournamentTowns())
            {
                if (!IsEligibleLord(party, tournamentTown))
                    return;

                var score = CalculateAttractionScore(party, tournamentTown.Settlement);
                if (score <= 0f)
                    return;

                AddTournamentBehaviorScore(p, tournamentTown.Settlement, score);
            }
        }

        private static List<Town> GetAllActiveTournamentTowns()
        {
            return JoustingCampaignBehavior.Instance.ActiveJoustingTournamentTown;
        }

        private static bool IsEligibleLord(MobileParty party, Town tournamentTown)
        {
            if (party.IsMainParty)
                return false;

            if (!party.IsLordParty)
                return false;

            if (party.LeaderHero == null)
                return false;

            if (party.MapFaction.IsAtWarWith(tournamentTown.Settlement.MapFaction))
                return false;

            return true;
        }

        private float CalculateAttractionScore(MobileParty party, Settlement tournamentSettlement)
        {
            var distanceScore = GetDistanceScore(party, tournamentSettlement);
            var relationScore = GetRelationScore(party, tournamentSettlement);
            var militaryScore = GetMilitaryScore(party);
            var ownerScore = GetOwnerBonusScore(party, tournamentSettlement);
            var groomScore = GetGroomBonusScore(party);

            return MathF.Max(0f, distanceScore + relationScore + militaryScore + ownerScore + groomScore);
        }

        private static float GetOwnerBonusScore(MobileParty party, Settlement tournamentSettlement)
        {
            if (party.Army != null)
                return 0f;

            if (party.LeaderHero?.Clan == tournamentSettlement.OwnerClan)
                return JoustingConfig.OwnerBonusScore;

            return 0f;
        }

        private static float GetGroomBonusScore(MobileParty party)
        {
            if (party.LeaderHero == null)
                return 0f;

            if (party.Army != null)
                return 0f;

            var joustingBehavior = JoustingCampaignBehavior.Instance;
            if (joustingBehavior.IsRecentlyWedGroom(party.LeaderHero))
                return JoustingConfig.GroomBonusScore;

            return 0f;
        }

        private static float GetDistanceScore(MobileParty party, Settlement tournamentSettlement)
        {
            AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(party, tournamentSettlement, false, out _, out var distance, out _);

            if (distance > JoustingConfig.MaxDistanceForAttraction)
            {
                return -JoustingConfig.MaxDistanceScore;
            }
            return (1f - distance / JoustingConfig.MaxDistanceForAttraction) * JoustingConfig.MaxDistanceScore;
        }

        private static float GetRelationScore(MobileParty party, Settlement tournamentSettlement)
        {
            var partyLeader = party.LeaderHero;
            var settlementOwner = tournamentSettlement.OwnerClan?.Leader;

            if (partyLeader == null || settlementOwner == null)
            {
                return 0f;
            }

            var relation = partyLeader.GetRelation(settlementOwner);

            return (relation / 100f) * JoustingConfig.MaxRelationScore;
        }

        private static float GetMilitaryScore(MobileParty party)
        {
            if (party.Army != null)
            {
                return -JoustingConfig.MaxMilitaryPenalty;
            }

            if (party.MapFaction is not Kingdom kingdom)
            {
                return 0f;
            }

            var activeArmyCount = kingdom.Armies.Count;

            return -MathF.Min(activeArmyCount * JoustingConfig.ArmyPenaltyPerArmy, JoustingConfig.MaxMilitaryPenalty);
        }

        private static void AddTournamentBehaviorScore(PartyThinkParams p, Settlement settlement, float score)
        {
            var data = new AIBehaviorData(settlement, AiBehavior.GoToSettlement, MobileParty.NavigationType.All, false, false, false);

            if (p.TryGetBehaviorScore(data, out var existingScore))
            {
                p.SetBehaviorScore(data, existingScore + score);
                return;
            }

            p.AddBehaviorScore(new ValueTuple<AIBehaviorData, float>(data, score));
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
