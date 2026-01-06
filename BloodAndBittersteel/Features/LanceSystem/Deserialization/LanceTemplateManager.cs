using System.Collections.Generic;
using System.Linq;

namespace BloodAndBittersteel.Features.LanceSystem.Deserialization
{
    public class LanceTemplateManager
    {
        private static readonly string _path = PathHelper.BaBOutsideConfigPath + "lance_templates.xml";
        private static LanceTemplateManager? _instance;
        public static LanceTemplateManager Instance => _instance ??= new LanceTemplateManager();

        public LanceTemplates Templates { get; private set; } = new();

        private LanceTemplateManager() { }

        public void LoadFromFile()
        {
            Templates = LanceDataDeserializer.LoadFromFile(_path);
        }

        public IEnumerable<Lance> GetLances(string cultureId, LanceTemplateSettlementType settlementType)
        {
            return Templates.Lances
                .Where(l => l.CultureId == cultureId &&
                            (l.SettlementType == settlementType || l.SettlementType == LanceTemplateSettlementType.All));
        }
    }
}
