using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using static LanceSystem.DynamicTroops.UI.ItemSelection.Filters.FilterFactory;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public class ObjectSelectorController<T> where T : MBObjectBase
    {
        const int ItemsPerRow = 3;
        readonly List<T> _allItems;
        readonly Action<T> _apply;
        readonly Func<T, CardVM> _cardFactory;
        readonly List<FilterDefinition<T>> _filters;
        GauntletLayer? _layer;
        GauntletMovieIdentifier? _movie;
        public ObjectSelectorVM? Vm { get; private set; }

        public ObjectSelectorController(List<T> items, Action<T> apply, Func<T, Action?, CardVM> cardFactory, List<FilterDefinition<T>> filters)
        {
            _allItems = new List<T>(items);
            _apply = apply;
            _cardFactory = (item) => cardFactory(item, Close);
            _filters = filters;
            foreach (var filter in _filters)
                filter.Context.OnChanged += ApplyFilters;
        }

        public void Open()
        {
            var rows = BuildRows(GetFilteredItems());
            var filterVms = GetFilterViewModels();
            Vm = new ObjectSelectorVM(rows, filterVms, ClearFilters, ApplyFilters, Close);
            _layer = new GauntletLayer("ObjectSelectorLayer", 1001);
            _movie = _layer.LoadMovie("ObjectSelection", Vm);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            ScreenManager.TopScreen.AddLayer(_layer);
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }

        public void Close()
        {
            if (_layer == null) return;
            _layer.InputRestrictions.ResetInputRestrictions();
            _layer.IsFocusLayer = false;
            if (_movie != null) _layer.ReleaseMovie(_movie);
            ScreenManager.TopScreen.RemoveLayer(_layer);
            _layer = null;
            _movie = null;
            Vm = null;
        }

        public IList<T> GetFilteredItems()
        {
            IList<T> result = _allItems;
            for (int i = _filters.Count - 1; i >= 0; i--)
                result = _filters[i].Filter.GetFilteredItems(result);
            return result;
        }

        MBBindingList<ObjectRowVM> BuildRows(IList<T> items)
        {
            MBBindingList<ObjectRowVM> rows = new();
            MBBindingList<CardVM> current = new();
            foreach (T item in items)
            {
                current.Add(_cardFactory(item));
                if (current.Count == ItemsPerRow)
                {
                    rows.Add(new ObjectRowVM(current));
                    current = new MBBindingList<CardVM>();
                }
            }
            if (current.Count > 0) rows.Add(new ObjectRowVM(current));
            return rows;
        }

        MBBindingList<FilterViewModel> GetFilterViewModels()
        {
            MBBindingList<FilterViewModel> vms = new();
            foreach (var filter in _filters)
                vms.Add(filter.ViewModel);
            return vms;
        }

        void ClearFilters()
        {
            foreach (var filter in _filters)
                filter.Context.Reset();
            Vm?.SetItems(BuildRows(GetFilteredItems()));
        }

        void ApplyFilters()
        {
            Vm?.SetItems(BuildRows(GetFilteredItems()));
        }

        public void Apply(T item)
        {
            _apply?.Invoke(item);
            Close();
        }
    }
}
