using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.NameFilters
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
        public IList<T> Filter(IList<T> data)
        {
            if (string.IsNullOrWhiteSpace(_context.Query)) return data;
            return data.Where(item => (_nameSelector(item) ?? "").IndexOf(_context.Query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
    }
}
