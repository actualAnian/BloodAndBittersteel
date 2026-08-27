using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.NameFilters
{
    public class NameContext : IFilterContext
    {
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
        }

        public void Reset()
        {
            _text = "";
        }
    }
}
