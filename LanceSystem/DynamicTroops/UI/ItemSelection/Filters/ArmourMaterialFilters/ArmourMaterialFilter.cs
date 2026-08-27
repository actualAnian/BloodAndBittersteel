using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.ArmourMaterialFilters
{
    public class ArmourMaterialFilter : IDataFilter<ItemObject>
    {
        readonly ArmourMaterialContext _context;
        public ArmourMaterialFilter(ArmourMaterialContext context)
        {
            _context = context;
        }
        public IList<ItemObject> Filter(IList<ItemObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => item?.ArmorComponent == null || _context.Selected.Contains(item.ArmorComponent.MaterialType)).ToList();
        }
    }
}
