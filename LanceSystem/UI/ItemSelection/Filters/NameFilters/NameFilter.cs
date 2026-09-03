using LanceSystem.SimpleFuzzySearch;
using System;
using System.Collections.Generic;
using System.Linq;
namespace LanceSystem.UI.ItemSelection.Filters.NameFilters
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
        public IEnumerable<T> GetFilteredItems(IEnumerable<T> data)
        {
            if (string.IsNullOrWhiteSpace(_context.Query))
                return data.OrderBy(_nameSelector);
            return FuzzySearchManager.TrySearchOrdered(data, _context.Query, item => _nameSelector(item));
        }
    }
}
