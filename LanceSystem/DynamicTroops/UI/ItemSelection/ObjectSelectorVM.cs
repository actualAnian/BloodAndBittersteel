using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;
using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public class ObjectSelectorVM : ViewModel
    {
        static readonly TextObject _filtersHeaderText = new("{=lance_filters_header}Filters");
        static readonly TextObject _clearFiltersText = new("{=lance_clear_filters}Clear Filters");
        static readonly TextObject _leaveText = new("{=lance_leave}Leave");
        readonly Action _clearFilters;
        readonly Action _applyFilters;
        string _titleText;
        readonly Action _close;
        MBBindingList<ObjectRowVM> _rows = new();
        MBBindingList<FilterViewModel> _filters = new();
        public ObjectSelectorVM(string title, MBBindingList<ObjectRowVM> rows, MBBindingList<FilterViewModel> filters, Action clearFilters, Action applyFilters, Action close)
        {
            _titleText = title;
            _clearFilters = clearFilters;
            _applyFilters = applyFilters;
            _close = close;
            _rows = rows;
            _filters = filters;
        }
        [DataSourceProperty] public string TitleText { get => _titleText; set { if (value != _titleText) { _titleText = value; OnPropertyChangedWithValue(value, "TitleText"); } } }
        [DataSourceProperty] public MBBindingList<ObjectRowVM> Rows { get => _rows; set { if (value != _rows) { _rows = value; OnPropertyChangedWithValue(value, "Rows"); } } }
        [DataSourceProperty] public MBBindingList<FilterViewModel> Filters { get => _filters; set { if (value != _filters) { _filters = value; OnPropertyChangedWithValue(value, "Filters"); } } }
        [DataSourceProperty] public string FiltersHeaderText => _filtersHeaderText.ToString();
        [DataSourceProperty] public string ClearFiltersButtonText => _clearFiltersText.ToString();
        [DataSourceProperty] public string LeaveButtonText => _leaveText.ToString();
        public void SetItems(MBBindingList<ObjectRowVM> rows) => Rows = rows;
        public void ExecuteClearFilters() => _clearFilters?.Invoke();
        public void ExecuteApplyFilters() => _applyFilters?.Invoke();
        public void Close() => _close?.Invoke();
    }
}
