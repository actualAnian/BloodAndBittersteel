using System.Collections.Generic;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.TierFilters
{
    public class TierContext : MultiSelectionContext
    {
        public override string Title => "Tier";
        public override string DisplayText => _selected.Count == 7 ? "All" : string.Join(", ", _selected);
        HashSet<int> _selected = new() { 0, 1, 2, 3, 4, 5, 6 };
        public HashSet<int> Selected => _selected;

        protected override List<InquiryElement> BuildElements()
        {
            List<InquiryElement> elements = new();
            for (int i = 0; i <= 6; i++)
                elements.Add(new InquiryElement(i.ToString(), "Tier " + i, null, true, ""));
            return elements;
        }

        protected override void ApplySelection(List<InquiryElement> selected)
        {
            List<int> values = new();
            foreach (InquiryElement element in selected)
                if (int.TryParse(element.Identifier as string, out int v)) values.Add(v);
            _selected = new HashSet<int>(values);
        }

        protected override void OnResetInternal()
        {
            _selected = new HashSet<int> { 0, 1, 2, 3, 4, 5, 6 };
        }
    }
}
