using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.WeaponTypeFilters;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.WeaponTypeFilters
{
    public class WeaponTypeFilter : IDataFilter<ItemObject>
    {
        readonly WeaponTypeContext _context;

        public WeaponTypeFilter(WeaponTypeContext context)
        {
            _context = context;
        }

        public IList<ItemObject> Filter(IList<ItemObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => item == null || !IsWeaponType(item.Type) || _context.Selected.Contains(item.Type)).ToList();
        }
        static bool IsWeaponType(ItemObject.ItemTypeEnum type) => (int)type - 2 <= 8 || (int)type - 16 <= 2;
    }
}
