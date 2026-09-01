using System.Collections.Generic;
using System.IO;
using LanceSystem.Deserialization;

namespace LanceSystem.DynamicLances
{
    public class DynamicLancesService
    {
        static DynamicLancesService? _instance;
        public static DynamicLancesService Instance => _instance ??= new DynamicLancesService(new DynamicLancesXmlFactory());

        readonly Dictionary<string, Lance> _lances = new();
        readonly IDynamicLancesFactory _factory;
        static readonly string _xmlPath = Path.Combine(PathHelper.OutsideConfigPath, "dynamic_lances.xml");
        public DynamicLancesService(IDynamicLancesFactory factory)
        {
            _factory = factory;
        }

        public void LoadLances()
        {
            if (!File.Exists(_xmlPath))
                return;
            var loaded = LanceDataDeserializer.LoadFromFile(_xmlPath);
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

        public Lance? SaveLance(string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            string stringId = name.ToLower().Replace(" ", "_");
            return _factory.SaveLance(stringId, name, cultureId, clanId, originType, troopsTemplate, weight, bannerKey);
        }
    }
}
