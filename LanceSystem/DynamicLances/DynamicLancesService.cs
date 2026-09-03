using System.Collections.Generic;
using System.IO;
using LanceSystem.Common;
using LanceSystem.Deserialization;

namespace LanceSystem.DynamicLances
{
    public class DynamicLancesService
    {
        static DynamicLancesService? _instance;
        public static DynamicLancesService Instance => _instance ??= new DynamicLancesService(new DynamicLancesXmlFactory());
        readonly Dictionary<string, Lance> _lances = new();
        readonly IDynamicLancesFactory _factory;
        public static readonly string XmlPath = Path.Combine(PathHelper.OutsideConfigPath, "dynamic_lances.xml");
        public DynamicLancesService(IDynamicLancesFactory factory)
        {
            _factory = factory;
        }
        public void LoadLances()
        {
            if (!File.Exists(XmlPath))
                return;
            var loaded = LanceDataDeserializer.LoadFromFile(XmlPath);
            foreach (var kvp in loaded)
                _lances[kvp.Key] = kvp.Value;
        }
        public bool IsDynamic(string stringId) => _lances.ContainsKey(stringId);
        public IReadOnlyDictionary<string, Lance> Lances => _lances;
        public IEnumerable<Lance> GetAllLances() => _lances.Values;
        public Lance? GetLance(string stringId)
        {
            _lances.TryGetValue(stringId, out var lance);
            return lance;
        }

        public void AddLance(Lance lance)
        {
            _lances[lance.StringId] = lance;
        }
        public void Reset() => _lances.Clear();
        public Result<Lance> SaveLance(string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Lance>.Fail("Lance name cannot be empty.");
            string stringId = name.ToLower().Replace(" ", "_");
            if (LanceTemplateManager.Instance.GetLanceFromId(stringId) != null)
                return Result<Lance>.Fail("Lance with the same ID already exists.");
            return Result<Lance>.Ok(_factory.SaveLance(stringId, name, cultureId, clanId, originType, troopsTemplate, weight, bannerKey));
        }
    }
}
