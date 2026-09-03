using System.IO;
using System.Xml;
using TaleWorlds.CampaignSystem;
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

        public CharacterObject CreateCharacterFromData(TroopEditorData data)
        {
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml(data);
            XmlDocument doc = new();
            doc.LoadXml(xml);
            CharacterObject character = (CharacterObject)MBObjectManager.Instance.CreateObjectFromXmlNode(doc.DocumentElement, "NPCCharacter");
            DynamicTroopsService.Instance.MarkDynamic(data.Name);
            _persistence.SaveToXml(data.Name, xml);
            return character;
        }

        public void UpdateCharacterFromData(TroopEditorData data)
        {
            CharacterObject existing = MBObjectManager.Instance.GetObject<CharacterObject>(data.Name);
            string xml = DynamicTroopsXmlHelper.BuildNpcCharacterXml(data);
            XmlDocument doc = new();
            doc.LoadXml(xml);
            existing.Deserialize(MBObjectManager.Instance, doc.DocumentElement);
            existing.AfterInitialized();
            DynamicTroopsService.Instance.MarkDynamic(data.Name);
            _persistence.SaveToXml(data.Name, xml);
        }
    }
}
