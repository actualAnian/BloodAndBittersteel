using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters
{
    public abstract class MultiSelectionContext : IFilterContext
    {
        public abstract string Title { get; }
        public abstract string DisplayText { get; }
        protected abstract List<InquiryElement> BuildElements();
        protected abstract void ApplySelection(List<InquiryElement> selected);
        protected abstract void OnReset();

        public void OnEventClicked()
        {
            List<InquiryElement> elements = BuildElements();
            if (elements.Count == 0) return;
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select " + Title, "", elements, true, 1, elements.Count, "Continue", null, args => ApplySelection(args.ToList()), null, "", false), false, false);
        }
        public void Reset() => OnReset();
    }
}
