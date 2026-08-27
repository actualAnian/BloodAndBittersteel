using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection
{
    public class ObjectSelectorVM : ViewModel
    {
        const int ItemsPerRow = 3;
        readonly CharacterObject _troop;
        readonly string _slotKey;
        readonly Action _clearFilters;
        readonly Action _applyFilters;
        readonly Action _close;
        MBBindingList<ObjectRowVM> _rows = new();
        MBBindingList<FilterViewModel> _filters = new();

        public ObjectSelectorVM(MBBindingList<ObjectRowVM> rows, MBBindingList<FilterViewModel> filters, CharacterObject troop, string slotKey, Action clearFilters, Action applyFilters, Action close)
        {
            _troop = troop;
            _slotKey = slotKey;
            _clearFilters = clearFilters;
            _applyFilters = applyFilters;
            _close = close;
            _rows = rows;
            _filters = filters;
        }

        [DataSourceProperty] public MBBindingList<ObjectRowVM> Rows { get => _rows; set { if (value != _rows) { _rows = value; OnPropertyChangedWithValue(value, "Rows"); } } }
        [DataSourceProperty] public MBBindingList<FilterViewModel> Filters { get => _filters; set { if (value != _filters) { _filters = value; OnPropertyChangedWithValue(value, "Filters"); } } }
        public void SetItems(MBBindingList<ObjectRowVM> rows) => Rows = rows;
        public void ExecuteClearFilters() => _clearFilters?.Invoke();
        public void ExecuteApplyFilters() => _applyFilters?.Invoke();
        public void Close() => _close?.Invoke();
    }
}
