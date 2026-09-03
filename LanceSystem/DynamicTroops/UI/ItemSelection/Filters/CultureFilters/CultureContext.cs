using System.Collections.Generic;
using System.Linq;
using LanceSystem.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CultureFilters
{
    public class CultureContext : MultiSelectionContext
    {
        HashSet<CultureObject> _selected = new();
        public IReadOnlyCollection<CultureObject> Selected => _selected;
        public override string Title => GameTexts.FindText("str_culture", null).ToString();
        public override string DisplayText => _selected.Count == 0 ? UITexts.All.ToString() : string.Join(", ", _selected.Select(c => c.Name.ToString()));

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
                    elements.Add(new InquiryElement(kingdom.Culture, kingdom.Culture.Name.ToString(), new BannerImageIdentifier(kingdom.Banner), true, ""));
                }
            if (elements.Count == 0) elements.Add(new InquiryElement(null, UITexts.NoCultures.ToString(), null));
            return elements;
        }

        protected override void OnResetInternal()
        {
            _selected = new HashSet<CultureObject>();
        }
    }
}
