using LanceSystem.LanceDataClasses;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace LanceSystem.Models
{
    public class LancePartySizeLimitModel : PartySizeLimitModel
    {
        PartySizeLimitModel _baseModel;
        public LancePartySizeLimitModel(PartySizeLimitModel baseModel)
        {
            _baseModel = baseModel;
        }
        public override int MinimumNumberOfVillagersAtVillagerParty => _baseModel.MinimumNumberOfVillagersAtVillagerParty;
        public override ExplainedNumber CalculateGarrisonPartySizeLimit(Settlement settlement, bool includeDescriptions = false)
        {
            return _baseModel.CalculateGarrisonPartySizeLimit(settlement, includeDescriptions);
        }

        public override TroopRoster FindAppropriateInitialRosterForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
        {
            return _baseModel.FindAppropriateInitialRosterForMobileParty(party, partyTemplate);
        }

        public override List<Ship> FindAppropriateInitialShipsForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
        {
            return _baseModel.FindAppropriateInitialShipsForMobileParty(party, partyTemplate);
        }

        public override int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan)
        {
            return _baseModel.GetAssumedPartySizeForLordParty(leaderHero, partyMapFaction, actualClan);
        }

        public override int GetClanTierPartySizeEffectForHero(Hero hero)
        {
            return _baseModel.GetClanTierPartySizeEffectForHero(hero);
        }

        public override int GetIdealVillagerPartySize(Village village)
        {
            return _baseModel.GetIdealVillagerPartySize(village);
        }
        public override int GetNextClanTierPartySizeEffectChangeForHero(Hero hero)
        {
            return _baseModel.GetNextClanTierPartySizeEffectChangeForHero(hero);
        }
        private bool IsUsingLanceSystem(PartyBase party)
        {
            return party == PartyBase.MainParty;
        }
        public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            if (!IsUsingLanceSystem(party))
                return _baseModel.GetPartyMemberSizeLimit(party, includeDescriptions);
            ExplainedNumber number = new();
            number.Add(Campaign.Current.Models.LanceModel().GetRetinueSizeLimit(party).RoundedResultNumber, new("{=lance_retinue_size}Retinue size"));
            foreach (LanceData lance in party.Lances())
            {
                number.Add(lance.TotalManCount, new("lance troops"));
            }
            return number;
        }
        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            return _baseModel.GetPartyPrisonerSizeLimit(party, includeDescriptions);
        }
    }
}