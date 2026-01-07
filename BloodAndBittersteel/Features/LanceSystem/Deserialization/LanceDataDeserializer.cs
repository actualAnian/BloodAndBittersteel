using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BaBUnitTests")]

namespace BloodAndBittersteel.Features.LanceSystem.Deserialization
{
    public static class LanceDataDeserializer
    {
        public static Dictionary<string, Lance> LoadFromFile(string xmlFilePath)
        {
            var xdoc = XDocument.Load(xmlFilePath);
            var templates = new Dictionary<string, Lance>();

            foreach (var lanceElem in xdoc.Root?.Elements("Lance") ?? Enumerable.Empty<XElement>())
            {
                var troopsElem = lanceElem.Element("Troops");
                var melee = ParseTroop(troopsElem.Element("MeleeTroop"));
                var ranged = ParseTroop(troopsElem.Element("RangedTroop"));
                var cavalry = ParseTroop(troopsElem.Element("CavalryTroop"));
                var horseArcher = ParseTroop(troopsElem.Element("HorseArcherTroop"));

                var parsed = new[] { melee, ranged, cavalry, horseArcher };
                var normalized = NormalizeTroopLikelihoods(parsed);

                var troops = new LanceTroopsTemplate(normalized[0], normalized[1], normalized[2], normalized[3]);

                var lance = new Lance(CleanString(lanceElem.Element("StringId").Value), CleanString(lanceElem.Element("CultureId").Value), ParseSettlementType(lanceElem.Element("SettlementType").Value), troops);

                templates.Add(lance.StringId, lance);
            }
            return templates;
        }
        private static TroopData ParseTroop(XElement troopElem)
        {
            return new TroopData(double.TryParse(troopElem.Element("Likelihood")?.Value, out var l) ? l : 0.0, CleanString(troopElem.Element("BasicTroopId")?.Value));
        }
        internal static TroopData[] NormalizeTroopLikelihoods(TroopData[] troops)
        {
            var nonNeg = troops.Select(t => new TroopData(t.Likelihood < 0.0 ? 0.0 : t.Likelihood, t.BasicTroopId)).ToArray();
            var sum = nonNeg.Sum(t => t.Likelihood);
            if (sum > 0.0)
            {
                return nonNeg.Select(t => new TroopData(t.Likelihood / sum, t.BasicTroopId)).ToArray();
            }
            var validCount = nonNeg.Count(t => !string.IsNullOrEmpty(t.BasicTroopId));
            if (validCount > 0)
            {
                var equal = 1.0 / validCount;
                return nonNeg.Select(t => !string.IsNullOrEmpty(t.BasicTroopId) ? new TroopData(equal, t.BasicTroopId) : new TroopData(0.0, t.BasicTroopId)).ToArray();
            }
            return nonNeg;
        }

        private static LanceTemplateSettlementType ParseSettlementType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return LanceTemplateSettlementType.All;

            value = value.Trim().Trim('"').ToLowerInvariant();

            return value switch
            {
                "town" => LanceTemplateSettlementType.Town,
                "settlement" => LanceTemplateSettlementType.Village,
                "castle" => LanceTemplateSettlementType.Castle,
                "all" => LanceTemplateSettlementType.All,
                _ => LanceTemplateSettlementType.All
            };
        }
        private static string CleanString(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value!.Trim().Trim('"');
        }
    }
}