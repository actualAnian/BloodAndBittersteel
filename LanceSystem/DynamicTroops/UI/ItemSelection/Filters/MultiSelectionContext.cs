using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters
{
    public abstract class MultiSelectionContext : IFilterContext
    {
        public event Action? OnChanged;
        public event Action? OnReset;

        public abstract string Title { get; }
        public abstract string DisplayText { get; }
        protected abstract List<InquiryElement> BuildElements();
        protected abstract void ApplySelection(List<InquiryElement> selected);
        public void OnEventClicked()
        {
            List<InquiryElement> elements = BuildElements();
            if (elements.Count == 0) return;
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select " + Title, "", elements, true, 1, elements.Count, "Continue", null, args =>
            {
                ApplySelection(args.ToList());
                OnChanged?.Invoke();
            }, null, "", false), false, false);
        }
        protected abstract void OnResetInternal();
        public void Reset() => OnReset?.Invoke();
    }
}
