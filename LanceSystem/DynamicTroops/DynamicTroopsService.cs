using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops
{
    public class DynamicTroopsService
    {
        static DynamicTroopsService? _instance;
        public static DynamicTroopsService Instance => _instance ??= new DynamicTroopsService(new DynamicTroopsXmlFactory());

        readonly HashSet<string> _dynamicIds = new();
        IDynamicTroopsFactory _factory;
        public DynamicTroopsService(IDynamicTroopsFactory factory)
        {
            _factory = factory;
        }
        public void MarkDynamic(string stringId)
        {
            _dynamicIds.Add(stringId);
        }

        public bool IsDynamic(string stringId)
        {
            return _dynamicIds.Contains(stringId);
        }

        public IReadOnlyCollection<string> DynamicIds => _dynamicIds;

        public void Reset()
        {
            _dynamicIds.Clear();
        }

        public CharacterObject? SaveCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate, Dictionary<SkillObject, int> skillValues)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            bool exists = MBObjectManager.Instance.GetObject<CharacterObject>(name) != null;
            if (exists)
            {
                _factory.UpdateCharacterFromData(name, isFemale, defaultGroup, tier, culture, upgradesTo, roster, faceKeyTemplate, skillValues);
                return MBObjectManager.Instance.GetObject<CharacterObject>(name);
            }
            return _factory.CreateCharacterFromData(name, isFemale, defaultGroup, tier, culture, upgradesTo, roster, faceKeyTemplate, skillValues);
        }
    }
}
