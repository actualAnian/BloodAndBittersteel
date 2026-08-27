using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.CultureFilters;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.CultureFilters
{
    public class ItemCultureFilter : IDataFilter<ItemObject>
    {
        CultureContext _context;
        public ItemCultureFilter(CultureContext context)
        {
            _context = context;
        }

        public IList<ItemObject> Filter(IList<ItemObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => item?.Culture != null && _context.Selected.Contains(item.Culture)).ToList();
        }
    }
}
