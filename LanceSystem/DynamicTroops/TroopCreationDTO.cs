using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops
{
    public record TroopCreationDTO
    {
        public TroopCreationDTO(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate, Dictionary<SkillObject, int> skillValues)
        {
            Name = name;
            IsFemale = isFemale;
            DefaultGroup = defaultGroup;
            Tier = tier;
            Culture = culture;
            UpgradesTo = upgradesTo;
            Roster = roster;
            FaceKeyTemplate = faceKeyTemplate;
            SkillValues = skillValues;
        }
        public string Name { get; init; }
        public bool IsFemale { get; init; }
        public FormationClass DefaultGroup { get; init; }
        public int Tier { get; init; }
        public CultureObject Culture { get; init; }
        public List<CharacterObject> UpgradesTo { get; init; }
        public MBEquipmentRoster Roster { get; init; }
        public MBBodyProperty? FaceKeyTemplate { get; init; }
        public Dictionary<SkillObject, int> SkillValues { get; init; }
    }
}
