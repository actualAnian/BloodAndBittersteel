using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.CultureFilters
{
    public class CultureContext : MultiSelectionContext
    {
        HashSet<CultureObject> _selected = new();
        public IReadOnlyCollection<CultureObject> Selected => _selected;
        public override string Title => "Culture";
        public override string DisplayText => _selected.Count == 0 ? "All" : string.Join(", ", _selected.Select(c => c.Name.ToString()));

        protected override void ApplySelection(List<InquiryElement> selected)
        {
            if (selected == null) return;
            List<CultureObject> values = new();
            foreach (InquiryElement element in selected)
                if (element.Identifier is CultureObject c) values.Add(c);
            _selected = new HashSet<CultureObject>(values);
        }

        protected override List<InquiryElement> BuildElements()
        {
            List<CultureObject> cultures = new();
            List<InquiryElement> elements = new();
            if (Campaign.Current != null)
                foreach (Kingdom kingdom in Campaign.Current.Kingdoms)
                {
                    if (kingdom?.Culture == null || cultures.Contains(kingdom.Culture)) continue;
                    cultures.Add(kingdom.Culture);
                    elements.Add(new InquiryElement(kingdom.Culture, kingdom.Culture.Name.ToString(), new BannerImageIdentifier(kingdom.Banner), true, ""));//_selected.Contains(kingdom.Culture), ""));
                }
            if (elements.Count == 0) elements.Add(new InquiryElement(null, "No cultures", null));
            return elements;
        }

        protected override void OnReset()
        {
            _selected = new HashSet<CultureObject>();
        }
    }
}
