using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.LanceSystem.Deserialization
{
    public class LanceTemplateManager
    {
        private static readonly string _path = PathHelper.BaBOutsideConfigPath + "lance_templates.xml";
        private static LanceTemplateManager? _instance;
        public static LanceTemplateManager Instance => _instance ??= new LanceTemplateManager();
        public Dictionary<string, Lance> Lances { get; private set; } = new();
        private readonly Lance FallBackLance = new("fallback", "any", LanceTemplateSettlementType.All,
            new(new TroopData(0.5, "looter"), new TroopData(0.5, "looter"), new TroopData(0, "looter"), new TroopData(0, "looter")));

        private LanceTemplateManager() { }

        public void LoadFromFile()
        {
            Lances = LanceDataDeserializer.LoadFromFile(_path);
        }
        public IEnumerable<Lance> GetLances(string cultureId, LanceTemplateSettlementType settlementType)
        {
            var result = Lances.Values.Where(l =>
                l.CultureId == cultureId &&
                (l.SettlementType == settlementType ||
                 l.SettlementType == LanceTemplateSettlementType.All));
            return result.Any() ? result : new List<Lance> { FallBackLance };
        }
        public Lance GetLanceFromId(string lanceId)
        {
            Lances.TryGetValue(lanceId, out var lance);
            if (lance == null)
                InformationManager.DisplayMessage(new($"Error, lance with id {lanceId} does not exist!"));
            return lance ?? FallBackLance;
        }
    }
}
