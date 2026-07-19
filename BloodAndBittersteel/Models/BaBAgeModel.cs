using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace BloodAndBittersteel.Models
{
    internal class BaBAgeModel : AgeModel
    {
        private AgeModel _baseModel;

        public BaBAgeModel(AgeModel baseModel)
        {
            _baseModel = baseModel;
        }

        public override int BecomeInfantAge => _baseModel.BecomeInfantAge;

        public override int BecomeChildAge => _baseModel.BecomeChildAge;

        public override int BecomeTeenagerAge => _baseModel.BecomeTeenagerAge;

        public override int HeroComesOfAge => 16;

        public override int BecomeOldAge => _baseModel.BecomeOldAge;

        public override int MiddleAdultHoodAge => _baseModel.MiddleAdultHoodAge;

        public override int MaxAge => _baseModel.MaxAge;

        public override void GetAgeLimitForLocation(CharacterObject character, out int minimumAge, out int maximumAge, string additionalTags = "")
        {
            _baseModel.GetAgeLimitForLocation(character, out minimumAge, out maximumAge, additionalTags);
        }
    }
}
