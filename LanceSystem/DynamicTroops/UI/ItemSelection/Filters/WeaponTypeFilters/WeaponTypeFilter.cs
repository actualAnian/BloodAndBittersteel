using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.WeaponTypeFilters
{
    public class WeaponTypeFilter : IDataFilter<ItemObject>
    {
        readonly WeaponTypeContext _context;

        public WeaponTypeFilter(WeaponTypeContext context)
        {
            _context = context;
        }
        public IEnumerable<ItemObject> GetFilteredItems(IEnumerable<ItemObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(item => item == null || !IsWeaponType(item.Type) || _context.Selected.Contains(item.Type));
        }
        static bool IsWeaponType(ItemObject.ItemTypeEnum type)
        {
            return type == ItemObject.ItemTypeEnum.OneHandedWeapon
                || type == ItemObject.ItemTypeEnum.TwoHandedWeapon
                || type == ItemObject.ItemTypeEnum.Polearm
                || type == ItemObject.ItemTypeEnum.Arrows
                || type == ItemObject.ItemTypeEnum.Bolts
                || type == ItemObject.ItemTypeEnum.Bow
                || type == ItemObject.ItemTypeEnum.Crossbow
                || type == ItemObject.ItemTypeEnum.Shield
                || type == ItemObject.ItemTypeEnum.Thrown
                || type == ItemObject.ItemTypeEnum.Goods
                || type == ItemObject.ItemTypeEnum.Pistol
                || type == ItemObject.ItemTypeEnum.Musket
                || type == ItemObject.ItemTypeEnum.Bullets;
        }
    }
}
