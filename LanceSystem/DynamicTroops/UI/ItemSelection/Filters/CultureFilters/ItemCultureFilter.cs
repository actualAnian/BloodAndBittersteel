using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CultureFilters
{
    public class ItemCultureFilter : IDataFilter<ItemObject>
    {
        CultureContext _context;
        public ItemCultureFilter(CultureContext context)
        {
            _context = context;
        }

        public IList<ItemObject> GetFilteredItems(IList<ItemObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => item?.Culture != null && _context.Selected.Contains(item.Culture)).ToList();
        }
    }
}
