using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops
{
    public interface IDynamicTroopsFactory
    {
        CharacterObject CreateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate, Dictionary<SkillObject, int> skillValues);
        void UpdateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate, Dictionary<SkillObject, int> skillValues);
    }    
}
