using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace BaBUnitTests.TestUtilities
{
    internal class TestCharacterStatsModel : CharacterStatsModel
    {
        public override int MaxCharacterTier => throw new NotImplementedException();

        public override int GetTier(CharacterObject character)
        {
            return character.Level;
        }

        public override ExplainedNumber MaxHitpoints(CharacterObject character, bool includeDescriptions = false)
        {
            throw new NotImplementedException();
        }

        public override int WoundedHitPointLimit(Hero hero)
        {
            throw new NotImplementedException();
        }
    }
}
