using System.IO;
using System.Xml;

namespace LanceSystem.DynamicTroops
{
    public class DynamicTroopsXmlSaver
    {
        private readonly string _path;
        public string FilePath => _path;

        public DynamicTroopsXmlSaver(string path)
        {
            _path = path;
        }

        public void CreateCharacterXmlIfNeeded()
        {
            string dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(_path))
                return;
            XmlDocument doc = new();
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);
            XmlElement root = doc.CreateElement("NPCCharacters");
            doc.AppendChild(root);
            doc.Save(_path);
        }

        public void LoadAndMarkDynamic()
        {
            if (!File.Exists(_path))
                return;
            XmlDocument doc = new();
            doc.Load(_path);
            XmlElement root = doc.DocumentElement;
            if (root == null)
                return;
            foreach (XmlNode child in root.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;
                XmlAttribute idAttr = child.Attributes["id"];
                if (idAttr != null)
                    DynamicTroopsService.Instance.MarkDynamic(idAttr.Value);
            }
        }

        public void SaveToXml(string name, string npcCharacterXml)
        {
            CreateCharacterXmlIfNeeded();
            XmlDocument doc = new();
            doc.Load(_path);
            XmlElement root = doc.DocumentElement;
            if (root == null)
            {
                root = doc.CreateElement("NPCCharacters");
                doc.AppendChild(root);
            }
            XmlNode? existing = null;
            foreach (XmlNode child in root.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;
                XmlAttribute idAttr = child.Attributes["id"];
                if (idAttr != null && idAttr.Value == name)
                {
                    existing = child;
                    break;
                }
            }
            if (existing != null)
                root.RemoveChild(existing);
            XmlDocument fragmentDoc = new();
            fragmentDoc.LoadXml(npcCharacterXml);
            XmlNode imported = doc.ImportNode(fragmentDoc.DocumentElement, true);
            root.AppendChild(imported);
            doc.Save(_path);
        }
    }
}
