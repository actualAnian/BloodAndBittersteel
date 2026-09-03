using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.UI.ItemSelection.Filters.ArmourMaterialFilters
{
    public class ArmourMaterialFilter : IDataFilter<ItemObject>
    {
        readonly ArmourMaterialContext _context;
        public ArmourMaterialFilter(ArmourMaterialContext context)
        {
            _context = context;
        }
        public IEnumerable<ItemObject> GetFilteredItems(IEnumerable<ItemObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => item.ArmorComponent != null && _context.Selected.Contains(item.ArmorComponent.MaterialType));
        }
    }
}
