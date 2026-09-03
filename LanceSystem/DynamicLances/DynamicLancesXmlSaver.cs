using System.IO;
using System.Xml;

namespace LanceSystem.DynamicLances
{
    public class DynamicLancesXmlSaver
    {
        private readonly string _path;

        public DynamicLancesXmlSaver(string path)
        {
            _path = path;
        }

        public void CreateLanceXmlIfNeeded()
        {
            string dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (File.Exists(_path))
                return;
            XmlDocument doc = new();
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);
            XmlElement root = doc.CreateElement("LanceTemplates");
            doc.AppendChild(root);
            doc.Save(_path);
        }

        public void SaveToXml(string stringId, string lanceXml)
        {
            CreateLanceXmlIfNeeded();
            XmlDocument doc = new();
            doc.Load(_path);
            XmlElement root = doc.DocumentElement;
            if (root == null)
            {
                root = doc.CreateElement("LanceTemplates");
                doc.AppendChild(root);
            }
            XmlNode? existing = null;
            foreach (XmlNode child in root.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;
                XmlNode? idNode = child.SelectSingleNode("StringId");
                if (idNode != null && idNode.InnerText.Trim().Trim('"') == stringId)
                {
                    existing = child;
                    break;
                }
            }
            if (existing != null)
                root.RemoveChild(existing);
            XmlDocument fragmentDoc = new();
            fragmentDoc.LoadXml(lanceXml);
            XmlNode imported = doc.ImportNode(fragmentDoc.DocumentElement, true);
            root.AppendChild(imported);
            doc.Save(_path);
        }
    }
}
