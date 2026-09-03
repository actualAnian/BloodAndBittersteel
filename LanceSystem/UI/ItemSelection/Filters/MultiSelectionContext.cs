using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LanceSystem.UI.ItemSelection.Filters
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
            MBTextManager.SetTextVariable("FILTER_TITLE", Title);
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=lance_select_filter_full}{FILTER_TITLE}").ToString(), "", elements, true, 1, elements.Count, new TextObject("{=WVkc4UgX}Continue.", null).ToString(), null, args =>
            {
                ApplySelection(args.ToList());
                OnChanged?.Invoke();
            }, null, "", false), false, false);
        }
        protected abstract void OnResetInternal();
        public void Reset()
        {
            OnReset?.Invoke();
            OnResetInternal();
        }
    }
}
