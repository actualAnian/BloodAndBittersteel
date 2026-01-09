using BloodAndBittersteel.Features.LanceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace BloodAndBittersteel.Models
{
    internal class BaBPartySizeLimitModel : PartySizeLimitModel
    {
        PartySizeLimitModel _baseModel;

        public BaBPartySizeLimitModel(PartySizeLimitModel baseModel)
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
        public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            if(!IsUsingLanceSystem(party))
                return _baseModel.GetPartyMemberSizeLimit(party, includeDescriptions);
            ExplainedNumber number = new();
            number.Add(MaxMainPartySize(party.Owner.Clan.Tier), new("{=bab_lance_party_size_base}From clan tier"));
            foreach (LanceData lance in party.Lances())
            {
                var behavior = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
                var notableLanceData = behavior.GetNotableData(lance.NotableId);
                number.Add(notableLanceData.CachedMaxLanceTroops.ResultNumber, notableLanceData.Name);
            }
            return number;
        }

        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            return _baseModel.GetPartyPrisonerSizeLimit(party, includeDescriptions);
        }
    }
}