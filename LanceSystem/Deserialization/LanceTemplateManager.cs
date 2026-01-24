using BloodAndBittersteel;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace LanceSystem.Deserialization
{
    public class LanceTemplateManager
    {
        private static readonly string _path = PathHelper.BaBOutsideConfigPath + "lance_templates.xml";
        private static LanceTemplateManager? _instance;
        public static LanceTemplateManager Instance => _instance ??= new LanceTemplateManager();
        public Dictionary<string, Lance> Lances { get; private set; } = new();
        private readonly Lance FallBackLance = new("fallback",
                                                    "Fallback Lance",
                                                   "any",
                                                   LanceTemplateOriginType.All,
                                                   new LanceTroopsTemplate(new List<TroopData>() { new(LanceTroopCategory.Infantry, 0.5, "looter"), new(LanceTroopCategory.Ranged, 0.5, "looter") }));
        private LanceTemplateManager() { }
        public void LoadFromFile()
        {
            Lances = LanceDataDeserializer.LoadFromFile(_path);
            Lances.Add("fallback", FallBackLance);
        }
        public IEnumerable<Lance> GetLances(string cultureId, LanceTemplateOriginType settlementType)
        {
            var result = Lances.Values.Where(l =>
                l.CultureId == cultureId &&
                (l.LanceOriginType == settlementType ||
                 l.LanceOriginType == LanceTemplateOriginType.All));
            return result.Any() ? result : new List<Lance> { FallBackLance };
        }
        public IEnumerable<Lance> GetLances(string cultureId, Settlement settlement)
        {
            LanceTemplateOriginType type;
            if (settlement.IsVillage) type = LanceTemplateOriginType.Village;
            else if (settlement.IsTown) type = LanceTemplateOriginType.Town;
            else type = LanceTemplateOriginType.Castle;
            return GetLances(cultureId, type);
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
