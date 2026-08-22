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

        public CharacterObject? CreateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            if (MBObjectManager.Instance.GetObject<CharacterObject>(name) != null)
                return null;
            return _factory.CreateCharacterFromData(name, isFemale, defaultGroup, tier, culture, upgradesTo, roster, faceKeyTemplate);
        }

        public bool UpdateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
        {
            CharacterObject existing = MBObjectManager.Instance.GetObject<CharacterObject>(name);
            if (existing == null)
                return false;
            _factory.UpdateCharacterFromData(name, isFemale, defaultGroup, tier, culture, upgradesTo, roster, faceKeyTemplate);
            return true;
        }
    }
}
