using System;
using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters
{
    public class FilterViewModel : ViewModel
    {
        readonly IFilterContext _context;
        readonly Action? _refresh;
        string _title;
        string _displayValue;

        public FilterViewModel(IFilterContext context, Action? refresh = null)
        {
            _context = context;
            _refresh = refresh;
            _title = _context.Title;
            _displayValue = _context.DisplayText;
        }

        [DataSourceProperty] public string Title { get => _title; set { if (value != _title) { _title = value; OnPropertyChangedWithValue(value, "Title"); } } }
        [DataSourceProperty] public string DisplayValue { get => _displayValue; set { if (value != _displayValue) { _displayValue = value; OnPropertyChangedWithValue(value, "DisplayValue"); } } }

        public void Refresh()
        {
            DisplayValue = _context.DisplayText;
            _refresh?.Invoke();
        }

        public void ExecuteFilter()
        {
            _context.OnEventClicked();
            Refresh();
        }
    }
}
