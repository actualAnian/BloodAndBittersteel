using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Models
{
    internal class BaBDiplomacyModel : DiplomacyModel
    {
        DiplomacyModel _baseModel;
        public BaBDiplomacyModel(DiplomacyModel baseModel)
        {
            _baseModel = baseModel;
        }

        public override int MaxRelationLimit => _baseModel.MaxRelationLimit;

        public override int MinRelationLimit => _baseModel.MinRelationLimit;

        public override int MaxNeutralRelationLimit => _baseModel.MaxNeutralRelationLimit;

        public override int MinNeutralRelationLimit => _baseModel.MinNeutralRelationLimit;

        public override int MinimumRelationWithConversationCharacterToJoinKingdom => _baseModel.MinimumRelationWithConversationCharacterToJoinKingdom;

        public override int GiftingTownRelationshipBonus => _baseModel.GiftingTownRelationshipBonus;

        public override int GiftingCastleRelationshipBonus => _baseModel.GiftingCastleRelationshipBonus;

        public override float WarDeclarationScorePenaltyAgainstAllies => _baseModel.WarDeclarationScorePenaltyAgainstAllies;

        public override float WarDeclarationScoreBonusAgainstEnemiesOfAllies => _baseModel.WarDeclarationScoreBonusAgainstEnemiesOfAllies;

        public override bool CanSettlementBeGifted(Settlement settlement)
        {
            return _baseModel.CanSettlementBeGifted(settlement);
        }

        public override float DenarsToInfluence()
        {
            return _baseModel.DenarsToInfluence();
        }

        public override IEnumerable<BarterGroup> GetBarterGroups()
        {
            return _baseModel.GetBarterGroups();
        }

        public override int GetBaseRelation(Hero hero, Hero hero1)
        {
            return _baseModel.GetBaseRelation(hero, hero1);
        }

        public override int GetCharmExperienceFromRelationGain(Hero hero, float relationChange, ChangeRelationAction.ChangeRelationDetail detail)
        {
            return _baseModel.GetCharmExperienceFromRelationGain(hero, relationChange, detail);
        }

        public override float GetClanStrength(Clan clan)
        {
            return _baseModel.GetClanStrength(clan);
        }

        public override int GetDailyTributeToPay(Clan factionToPay, Clan factionToReceive, out int tributeDurationInDays)
        {
            return _baseModel.GetDailyTributeToPay(factionToPay, factionToReceive, out tributeDurationInDays);
        }

        public override float GetDecisionMakingThreshold(IFaction consideringFaction)
        {
            return _baseModel.GetDecisionMakingThreshold(consideringFaction);
        }

        public override DiplomacyStance GetDefaultDiplomaticStance(IFaction faction1, IFaction faction2)
        {
            return _baseModel.GetDefaultDiplomaticStance(faction1, faction2);
        }

        public override int GetEffectiveRelation(Hero hero, Hero hero1)
        {
            return _baseModel.GetEffectiveRelation(hero, hero1);
        }

        public override float GetHeroCommandingStrengthForClan(Hero hero)
        {
            return _baseModel.GetHeroCommandingStrengthForClan(hero);
        }

        public override void GetHeroesForEffectiveRelation(Hero hero1, Hero hero2, out Hero effectiveHero1, out Hero effectiveHero2)
        {
            _baseModel.GetHeroesForEffectiveRelation(hero1, hero2, out effectiveHero1, out effectiveHero2);
        }

        public override float GetHeroGoverningStrengthForClan(Hero hero)
        {
            return _baseModel.GetHeroGoverningStrengthForClan(hero);
        }

        public override float GetHourlyInfluenceAwardForBeingArmyMember(MobileParty mobileParty)
        {
            return _baseModel.GetHourlyInfluenceAwardForBeingArmyMember(mobileParty);
        }

        public override float GetHourlyInfluenceAwardForBesiegingEnemyFortification(MobileParty mobileParty)
        {
            return _baseModel.GetHourlyInfluenceAwardForBesiegingEnemyFortification(mobileParty);
        }

        public override float GetHourlyInfluenceAwardForRaidingEnemyVillage(MobileParty mobileParty)
        {
            return _baseModel.GetHourlyInfluenceAwardForRaidingEnemyVillage(mobileParty);
        }

        public override int GetInfluenceAwardForSettlementCapturer(Settlement settlement)
        {
            return _baseModel.GetInfluenceAwardForSettlementCapturer(settlement);
        }

        public override int GetInfluenceCostOfAbandoningArmy()
        {
            return _baseModel.GetInfluenceCostOfAbandoningArmy();
        }

        public override int GetInfluenceCostOfAnnexation(Clan proposingClan)
        {
            return _baseModel.GetInfluenceCostOfAnnexation(proposingClan);
        }

        public override int GetInfluenceCostOfChangingLeaderOfArmy()
        {
            return _baseModel.GetInfluenceCostOfChangingLeaderOfArmy();
        }

        public override int GetInfluenceCostOfDisbandingArmy()
        {
            return _baseModel.GetInfluenceCostOfDisbandingArmy();
        }

        public override int GetInfluenceCostOfExpellingClan(Clan proposingClan)
        {
            return _baseModel.GetInfluenceCostOfExpellingClan(proposingClan);
        }

        public override int GetInfluenceCostOfPolicyProposalAndDisavowal(Clan proposingClan)
        {
            return _baseModel.GetInfluenceCostOfPolicyProposalAndDisavowal(proposingClan);
        }

        public override int GetInfluenceCostOfProposingPeace(Clan proposingClan)
        {
            return _baseModel.GetInfluenceCostOfProposingPeace(proposingClan);
        }

        public override int GetInfluenceCostOfProposingWar(Clan proposingClan)
        {
            return _baseModel.GetInfluenceCostOfProposingWar(proposingClan);
        }

        public override int GetInfluenceCostOfSupportingClan()
        {
            return _baseModel.GetInfluenceCostOfSupportingClan();
        }

        public override int GetInfluenceValueOfSupportingClan()
        {
            return _baseModel.GetInfluenceValueOfSupportingClan();
        }

        public override uint GetNotificationColor(ChatNotificationType notificationType)
        {
            return _baseModel.GetNotificationColor(notificationType);
        }

        public override int GetRelationChangeAfterClanLeaderIsDead(Hero deadLeader, Hero relationHero)
        {
            return _baseModel.GetRelationChangeAfterClanLeaderIsDead(deadLeader, relationHero);
        }

        public override int GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(Hero supporter, bool hasHeroVotedAgainstOwner)
        {
            return _baseModel.GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(supporter, hasHeroVotedAgainstOwner);
        }

        public override int GetRelationCostOfDisbandingArmy(bool isLeaderParty)
        {
            return _baseModel.GetRelationCostOfDisbandingArmy(isLeaderParty);
        }

        public override int GetRelationCostOfExpellingClanFromKingdom()
        {
            return _baseModel.GetRelationCostOfExpellingClanFromKingdom();
        }

        public override float GetRelationIncreaseFactor(Hero hero1, Hero hero2, float relationValue)
        {
            return _baseModel.GetRelationIncreaseFactor(hero1, hero2, relationValue);
        }

        public override int GetRelationValueOfSupportingClan()
        {
            return _baseModel.GetRelationValueOfSupportingClan();
        }

        public override float GetScoreOfClanToJoinKingdom(Clan clan, Kingdom kingdom)
        {
            return _baseModel.GetScoreOfClanToJoinKingdom(clan, kingdom);
        }

        public override float GetScoreOfClanToLeaveKingdom(Clan clan, Kingdom kingdom)
        {
            return _baseModel.GetScoreOfClanToLeaveKingdom(clan, kingdom);
        }

        public override float GetScoreOfDeclaringPeace(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace)
        {
            return _baseModel.GetScoreOfDeclaringPeace(factionDeclaresPeace, factionDeclaredPeace);
        }

        public override float GetScoreOfDeclaringPeaceForClan(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace, Clan evaluatingClan, out TextObject reason, bool includeReason = false)
        {
            return _baseModel.GetScoreOfDeclaringPeaceForClan(factionDeclaresPeace, factionDeclaredPeace, evaluatingClan, out reason, includeReason);
        }

        public override float GetScoreOfDeclaringWar(IFaction factionDeclaresWar, IFaction factionDeclaredWar, Clan evaluatingClan, out TextObject reason, bool includeReason = false)
        {
            return _baseModel.GetScoreOfDeclaringWar(factionDeclaresWar, factionDeclaredWar, evaluatingClan, out reason, includeReason);
        }

        public override float GetScoreOfKingdomToGetClan(Kingdom kingdom, Clan clan)
        {
            return _baseModel.GetScoreOfKingdomToGetClan(kingdom, clan);
        }

        public override float GetScoreOfKingdomToHireMercenary(Kingdom kingdom, Clan mercenaryClan)
        {
            return _baseModel.GetScoreOfKingdomToHireMercenary(kingdom, mercenaryClan);
        }

        public override float GetScoreOfKingdomToSackClan(Kingdom kingdom, Clan clan)
        {
            return _baseModel.GetScoreOfKingdomToSackClan(kingdom, clan);
        }

        public override float GetScoreOfKingdomToSackMercenary(Kingdom kingdom, Clan mercenaryClan)
        {
            return _baseModel.GetScoreOfKingdomToSackMercenary(kingdom, mercenaryClan);
        }

        public override float GetScoreOfLettingPartyGo(MobileParty party, MobileParty partyToLetGo)
        {
            return _baseModel.GetScoreOfLettingPartyGo(party, partyToLetGo);
        }

        public override float GetScoreOfMercenaryToJoinKingdom(Clan clan, Kingdom kingdom)
        {
            return _baseModel.GetScoreOfMercenaryToJoinKingdom(clan, kingdom);
        }

        public override float GetScoreOfMercenaryToLeaveKingdom(Clan clan, Kingdom kingdom)
        {
            return _baseModel.GetScoreOfMercenaryToLeaveKingdom(clan, kingdom);
        }

        public override DiplomacyStance? GetShallowDiplomaticStance(IFaction faction1, IFaction faction2)
        {
            return _baseModel.GetShallowDiplomaticStance(faction1, faction2);
        }

        public override float GetStrengthThresholdForNonMutualWarsToBeIgnoredToJoinKingdom(Kingdom kingdomToJoin)
        {
            return _baseModel.GetStrengthThresholdForNonMutualWarsToBeIgnoredToJoinKingdom(kingdomToJoin);
        }

        public override float GetValueOfHeroForFaction(Hero examinedHero, IFaction targetFaction, bool forMarriage = false)
        {
            return _baseModel.GetValueOfHeroForFaction(examinedHero, targetFaction, forMarriage);
        }

        public override float GetValueOfSettlementsForFaction(IFaction faction)
        {
            return _baseModel.GetValueOfSettlementsForFaction(faction);
        }

        public override ExplainedNumber GetWarProgressScore(IFaction factionDeclaresWar, IFaction factionDeclaredWar, bool includeDescriptions = false)
        {
            return _baseModel.GetWarProgressScore(factionDeclaresWar, factionDeclaredWar, includeDescriptions);
        }

        public override bool IsAtConstantWar(IFaction faction1, IFaction faction2)
        {
            // @TODO check if this works as intended
            return _baseModel.IsAtConstantWar(faction1, faction2);
        }

        public override bool IsClanEligibleToBecomeRuler(Clan clan)
        {
            return _baseModel.IsClanEligibleToBecomeRuler(clan);
        }

        public override bool IsPeaceSuitable(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace)
        {
            return _baseModel.IsPeaceSuitable(factionDeclaresPeace, factionDeclaredPeace);
        }
    }
}
