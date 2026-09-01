using LanceSystem.SimpleFuzzySearch;
using System;
using System.Collections.Generic;
namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.NameFilters
{
    public class NameFilter<T> : IDataFilter<T>
    {
        readonly NameContext _context;
        readonly Func<T, string> _nameSelector;
        public NameFilter(NameContext context, Func<T, string> nameSelector)
        {
            _context = context;
            _nameSelector = nameSelector;
        }
        public IList<T> GetFilteredItems(IList<T> data)
        {
            if (string.IsNullOrWhiteSpace(_context.Query)) return data;
            return FuzzySearchManager.TrySearchOrdered<T>(data, _context.Query, item => _nameSelector(item));
        }
    }
}
