using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodAndBittersteel.Features.LanceSystem.Deserialization
{
    using System.Xml.Linq;

    public static class LanceDataDeserializer
    {
        public static LanceTemplates LoadFromFile(string xmlFilePath)
        {
            var xdoc = XDocument.Load(xmlFilePath);

            var templates = new LanceTemplates();

            foreach (var lanceElem in xdoc.Root?.Elements("Lance") ?? Enumerable.Empty<XElement>())
            {
                var lance = new Lance
                {
                    CultureId = CleanString(lanceElem.Element("CultureId")?.Value),
                    SettlementType = ParseSettlementType(lanceElem.Element("SettlementType")?.Value)
                };

                var troopsElem = lanceElem.Element("Troops");
                if (troopsElem != null)
                {
                    lance.Troops = new Troops
                    {
                        MeleeTroop = ParseTroop(troopsElem.Element("MeleeTroop")),
                        RangedTroop = ParseTroop(troopsElem.Element("RangedTroop")),
                        CavalryTroop = ParseTroop(troopsElem.Element("CavalryTroop")),
                        HorseArcherTroop = ParseTroop(troopsElem.Element("HorseArcherTroop")),
                    };
                }

                templates.Lances.Add(lance);
            }

            return templates;
        }

        private static TroopData ParseTroop(XElement? troopElem)
        {
            if (troopElem == null) return new TroopData();

            return new TroopData
            {
                Likelihood = double.TryParse(troopElem.Element("Likelihood")?.Value, out var l) ? l : 0.0,
                BasicTroopId = CleanString(troopElem.Element("BasicTroopId")?.Value)
            };
        }
        private static LanceTemplateSettlementType ParseSettlementType(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return LanceTemplateSettlementType.All;

            value = value.Trim().Trim('"').ToLowerInvariant();

            return value switch
            {
                "town" => LanceTemplateSettlementType.Town,
                "settlement" => LanceTemplateSettlementType.Settlement,
                "castle" => LanceTemplateSettlementType.Castle,
                "all" => LanceTemplateSettlementType.All,
                _ => LanceTemplateSettlementType.All // fallback if unknown
            };
        }
        private static string CleanString(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value!.Trim().Trim('"');
        }
    }

}
