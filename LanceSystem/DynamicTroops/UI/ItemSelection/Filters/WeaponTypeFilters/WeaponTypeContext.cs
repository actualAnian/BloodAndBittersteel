using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.WeaponTypeFilters
{
    public class WeaponTypeContext : MultiSelectionContext
    {
        static readonly Dictionary<string, ItemObject.ItemTypeEnum> Map = new()
        {
            { "Arrows", ItemObject.ItemTypeEnum.Arrows },
            { "Bolts", ItemObject.ItemTypeEnum.Bolts },
            { "Bow", ItemObject.ItemTypeEnum.Bow },
            { "Bullets", ItemObject.ItemTypeEnum.Bullets },
            { "Crossbow", ItemObject.ItemTypeEnum.Crossbow },
            { "Musket", ItemObject.ItemTypeEnum.Musket },
            { "One Handed Weapon", ItemObject.ItemTypeEnum.OneHandedWeapon },
            { "Pistol", ItemObject.ItemTypeEnum.Pistol },
            { "Polearm", ItemObject.ItemTypeEnum.Polearm },
            { "Shield", ItemObject.ItemTypeEnum.Shield },
            { "Thrown", ItemObject.ItemTypeEnum.Thrown },
            { "Two Handed Weapon", ItemObject.ItemTypeEnum.TwoHandedWeapon }
        };

        public override string Title => "Weapon";
        public override string DisplayText => _selected.Count == 0 ? "All" : string.Join(", ", _selected);
        HashSet<ItemObject.ItemTypeEnum> _selected = new();
        public IReadOnlyCollection<ItemObject.ItemTypeEnum> Selected => _selected;

        protected override List<InquiryElement> BuildElements()
        {
            return Map.Keys.Select(k => new InquiryElement(k, k, null, true, "")).ToList();
        }

        protected override void ApplySelection(List<InquiryElement> selected)
        {
            if (selected == null) return;
            List<ItemObject.ItemTypeEnum> values = new();
            foreach (InquiryElement element in selected)
                if (element.Identifier is string s && Map.ContainsKey(s)) values.Add(Map[s]);
            _selected = new HashSet<ItemObject.ItemTypeEnum>(values);
        }

        protected override void OnResetInternal()
        {
            _selected = new HashSet<ItemObject.ItemTypeEnum>();
        }
    }
}
