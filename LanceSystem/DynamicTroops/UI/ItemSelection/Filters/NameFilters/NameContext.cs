using System;
using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.NameFilters
{
    public class NameContext : IFilterContext
    {
        public event Action? OnChanged;
        public event Action? OnReset;

        string _text = "";
        public string Title => "Name";
        public string DisplayText => _text;
        public string Query => DisplayText;

        public void OnEventClicked()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData("Filter by name", "Type name substring", true, true, "Apply", "Cancel", OnTextEntered, null, false, null, ""), false, false);
        }

        void OnTextEntered(string text)
        {
            _text = text;
            OnChanged?.Invoke();
        }

        public void Reset()
        {
            _text = "";
            OnReset?.Invoke();
        }
    }
}
