using System.IO;
using System.Security;
using System.Text;
using LanceSystem.Deserialization;

namespace LanceSystem.DynamicLances
{
    public sealed class DynamicLancesXmlFactory : IDynamicLancesFactory
    {
        private readonly DynamicLancesXmlSaver _persistence;
        public DynamicLancesXmlFactory()
        {
            _persistence = new DynamicLancesXmlSaver(DynamicLancesService.XmlPath);
        }
        public Lance SaveLance(string stringId, string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey)
        {
            Lance lance = new(stringId, name, cultureId, clanId, originType, troopsTemplate, weight, bannerKey);
            string xml = BuildLanceXml(lance);
            DynamicLancesService.Instance.AddLance(lance);
            _persistence.SaveToXml(stringId, xml);
            return lance;
        }
        internal static string BuildLanceXml(Lance lance) =>
            BuildLanceXml(lance.StringId, lance.Name, lance.CultureId, lance.ClanId, lance.LanceOriginType, lance.TroopsTemplate, lance.weight, lance.bannerKey);
        internal static string BuildLanceXml(string stringId, string name, string? cultureId, string? clanId, LanceTemplateOriginType originType, LanceTroopsTemplate troopsTemplate, int weight, string? bannerKey)
        {
            StringBuilder sb = new();
            sb.Append("<Lance>");
            sb.Append($"<StringId>\"{XmlEscape(stringId)}\"</StringId>");
            sb.Append($"<Name>{XmlEscape(name)}</Name>");
            if (cultureId != null)
                sb.Append($"<CultureId>\"{XmlEscape(cultureId)}\"</CultureId>");
            if (clanId != null)
                sb.Append($"<ClanId>\"{XmlEscape(clanId)}\"</ClanId>");
            sb.Append($"<LanceOriginType>\"{originType.ToString().ToLowerInvariant()}\"</LanceOriginType>");
            sb.Append($"<Weight>{weight}</Weight>");
            if (bannerKey != null)
                sb.Append($"<BannerKey>\"{XmlEscape(bannerKey)}\"</BannerKey>");
            sb.Append(BuildTroopsXml(troopsTemplate));
            sb.Append("</Lance>");
            return sb.ToString();
        }
        static string BuildTroopsXml(LanceTroopsTemplate troopsTemplate)
        {
            StringBuilder sb = new();
            sb.Append("<Troops>");
            foreach (var troop in troopsTemplate.TroopTypes)
            {
                string elementName = TroopCategoryToElementName(troop.Category);
                sb.Append($"<{elementName}>");
                sb.Append($"<Likelihood>{troop.Likelihood}</Likelihood>");
                sb.Append($"<BasicTroopId>\"{XmlEscape(troop.BasicTroopId)}\"</BasicTroopId>");
                sb.Append($"</{elementName}>");
            }
            sb.Append("</Troops>");
            return sb.ToString();
        }
        static string TroopCategoryToElementName(LanceTroopCategory category)
        {
            return category switch
            {
                LanceTroopCategory.Infantry => "MeleeTroop",
                LanceTroopCategory.Ranged => "RangedTroop",
                LanceTroopCategory.Cavalry => "CavalryTroop",
                LanceTroopCategory.HorseArcher => "HorseArcherTroop",
                _ => "MeleeTroop"
            };
        }
        static string XmlEscape(string value)
        {
            return SecurityElement.Escape(value);
        }
    }
}
