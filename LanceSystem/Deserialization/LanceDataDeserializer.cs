using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("BaBUnitTests")]

namespace LanceSystem.Deserialization
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

                var parsed = new List<TroopData>();
                foreach (var elem in troopsElem.Elements())
                {
                    var category = ParseCategory(elem.Name.LocalName);
                    var likelihood = (double)elem.Element("Likelihood");
                    var basicTroopId = elem.Element("BasicTroopId")!.Value.Trim('"');

                    parsed.Add(new TroopData(category, likelihood, basicTroopId));
                }

                var normalized = NormalizeTroopLikelihoods(parsed);
                var troops = new LanceTroopsTemplate(normalized);
                var weight = (int?)lanceElem.Element("Weight") ?? 1;
                var lance = new Lance(CleanString(lanceElem.Element("StringId").Value),
                                      CleanString(lanceElem.Element("Name").Value),
                                      CleanString(lanceElem.Element("CultureId").Value),
                                      ParseSettlementType(lanceElem.Element("SettlementType").Value),
                                      troops, 
                                      weight
                                      );
                templates.Add(lance.StringId, lance);
            }
            return templates;
        }
        private static LanceTroopCategory ParseCategory(string elementName)
        {
            return elementName switch
            {
                "MeleeTroop" => LanceTroopCategory.Infantry,
                "RangedTroop" => LanceTroopCategory.Ranged,
                "CavalryTroop" => LanceTroopCategory.Cavalry,
                "HorseArcherTroop" => LanceTroopCategory.HorseArcher,
                _ => throw new InvalidOperationException($"Unknown troop type: {elementName}")
            };
        }
        public static List<TroopData> NormalizeTroopLikelihoods(IEnumerable<TroopData> troops)
        {
            var list = troops.ToList();
            var clamped = list
                .Select(t => t with { Likelihood = Math.Max(0, t.Likelihood) })
                .ToList();
            var sum = clamped.Sum(t => t.Likelihood);
            if (sum == 0)
            {
                var equal = 1.0 / clamped.Count;
                return clamped
                    .Select(t => t with { Likelihood = equal })
                    .ToList();
            }
            return clamped
                .Select(t => t with { Likelihood = t.Likelihood / sum })
                .ToList();
        }

        private static LanceTemplateOriginType ParseSettlementType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return LanceTemplateOriginType.All;

            value = value.Trim().Trim('"').ToLowerInvariant();

            return value switch
            {
                "town" => LanceTemplateOriginType.Town,
                "settlement" => LanceTemplateOriginType.Village,
                "castle" => LanceTemplateOriginType.Castle,
                "all" => LanceTemplateOriginType.All,
                _ => LanceTemplateOriginType.All
            };
        }
        private static string CleanString(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value!.Trim().Trim('"');
        }
    }
}