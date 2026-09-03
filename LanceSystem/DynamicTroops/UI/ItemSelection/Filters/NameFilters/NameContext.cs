using LanceSystem.UI;
using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

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
            InformationManager.ShowTextInquiry(new TextInquiryData(UITexts.FilterByName.ToString(), UITexts.TypeNameSubstring.ToString(), true, true, UITexts.Apply.ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), OnTextEntered, null, false, null, ""), false, false);
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
