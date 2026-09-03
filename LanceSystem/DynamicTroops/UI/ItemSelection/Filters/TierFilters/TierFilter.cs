using System;
using System.Collections.Generic;
using System.Linq;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.TierFilters
{
    public class TierFilter<T> : IDataFilter<T>
    {
        readonly TierContext _context;
        readonly Func<T, int> _tierSelector;
        public TierFilter(TierContext context, Func<T, int> tierSelector)
        {
            _context = context;
            _tierSelector = tierSelector;
        }
        public IEnumerable<T> GetFilteredItems(IEnumerable<T> data)
        {
            if (_context.Selected.Count == 7) return data;
            return data.Where(item => _context.Selected.Contains(_tierSelector(item)));
        }
    }
}
