using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.TierFilters;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.TierFilters
{
    public class TierFilter : IDataFilter<ItemObject>
    {
        readonly TierContext _context;
        public TierFilter(TierContext context) 
        {
            _context = context;
        }
        public IList<ItemObject> Filter(IList<ItemObject> data)
        {
            if (_context.Selected.Count == 7) return data;
            return data.Where(item => item == null || _context.Selected.Contains((int)item.Tier) || _context.Selected.Contains((int)item.Tier + 1)).ToList();
        }
    }
}
