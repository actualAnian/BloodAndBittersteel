using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.UI.ItemSelection.Filters.CultureFilters
{
    public class CultureFilter<T> : IDataFilter<T>
    {
        readonly CultureContext _context;
        readonly Func<T, CultureObject?> _cultureSelector;
        public CultureFilter(CultureContext context, Func<T, CultureObject?> cultureSelector)
        {
            _context = context;
            _cultureSelector = cultureSelector;
        }
        public IEnumerable<T> GetFilteredItems(IEnumerable<T> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => { var culture = _cultureSelector(item); return culture != null && _context.Selected.Contains(culture); });
        }
    }
}
