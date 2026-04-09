using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace BloodAndBittersteel.Models
{
    internal class BaBMarriageModel : MarriageModel
    {
        private readonly MarriageModel _baseModel;

        public BaBMarriageModel(MarriageModel baseModel)
        {
            _baseModel = baseModel;
        }

        public override int MinimumMarriageAgeMale => _baseModel.MinimumMarriageAgeMale;

        public override int MinimumMarriageAgeFemale => _baseModel.MinimumMarriageAgeFemale;

        public override List<Hero> GetAdultChildrenSuitableForMarriage(Hero hero)
        {
            return _baseModel.GetAdultChildrenSuitableForMarriage(hero);
        }

        public override Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero)
        {
            return _baseModel.GetClanAfterMarriage(firstHero, secondHero);
        }

        public override int GetEffectiveRelationIncrease(Hero firstHero, Hero secondHero)
        {
            return _baseModel.GetEffectiveRelationIncrease(firstHero, secondHero);
        }

        public override bool IsClanSuitableForMarriage(Clan clan)
        {
            return _baseModel.IsClanSuitableForMarriage(clan);
        }

        public override bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero)
        {
            if (firstHero.Culture.StringId == Globals.IronbornCultureId
                && secondHero.IsPrisoner
                && secondHero.PartyBelongedToAsPrisoner.MapFaction == firstHero.MapFaction)
                return true;
            return _baseModel.IsCoupleSuitableForMarriage(firstHero, secondHero);
        }

        public override bool IsSuitableForMarriage(Hero hero)
        {
            return _baseModel.IsSuitableForMarriage(hero);
        }

        public override float NpcCoupleMarriageChance(Hero firstHero, Hero secondHero)
        {
            return _baseModel.NpcCoupleMarriageChance(firstHero, secondHero);
        }

        public override bool ShouldNpcMarriageBetweenClansBeAllowed(Clan consideringClan, Clan targetClan)
        {
            return _baseModel.ShouldNpcMarriageBetweenClansBeAllowed(consideringClan, targetClan);
        }
    }
}
