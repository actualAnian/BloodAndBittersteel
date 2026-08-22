using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops
{
    public sealed class DynamicTroopsXmlFactory : IDynamicTroopsFactory
    {
        private readonly DynamicTroopsXmlSaver _persistence;
        static readonly string _xmlPath = Path.Combine(PathHelper.OutsideConfigPath, "dynamic_troops.xml");
        public DynamicTroopsXmlFactory()
        {
            _persistence = new DynamicTroopsXmlSaver(_xmlPath);
        }

        public CharacterObject CreateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml(name, isFemale, defaultGroup, tier, culture, upgradesTo, roster, faceKeyTemplate);
            XmlDocument doc = new();
            doc.LoadXml(xml);
            CharacterObject character = (CharacterObject)MBObjectManager.Instance.CreateObjectFromXmlNode(doc.DocumentElement, "NPCCharacter");
            DynamicTroopsService.Instance.MarkDynamic(name);
            _persistence.SaveToXml(name, xml);
            return character;
        }

        public void UpdateCharacterFromData(string name, bool isFemale, FormationClass defaultGroup, int tier, CultureObject culture, List<CharacterObject> upgradesTo, MBEquipmentRoster roster, MBBodyProperty? faceKeyTemplate)
        {
            CharacterObject existing = MBObjectManager.Instance.GetObject<CharacterObject>(name);
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml(name, isFemale, defaultGroup, tier, culture, upgradesTo, roster, faceKeyTemplate);
            XmlDocument doc = new();
            doc.LoadXml(xml);
            existing.Deserialize(MBObjectManager.Instance, doc.DocumentElement);
            existing.AfterInitialized();
            DynamicTroopsService.Instance.MarkDynamic(name);
            _persistence.SaveToXml(name, xml);
        }
    }
}
