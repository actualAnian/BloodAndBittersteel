using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.ArmourMaterialFilters
{
    public class ArmourMaterialContext : MultiSelectionContext
    {
        static readonly Dictionary<string, ArmorComponent.ArmorMaterialTypes> Map = new()
        {
            { "Chainmail", (ArmorComponent.ArmorMaterialTypes)3 },
            { "Cloth", (ArmorComponent.ArmorMaterialTypes)1 },
            { "Leather", (ArmorComponent.ArmorMaterialTypes)2 },
            { "Plate", (ArmorComponent.ArmorMaterialTypes)4 }
        };

        public override string Title => "Armour Material";
        public override string DisplayText => _selected.Count == 0 ? "All" : string.Join(", ", _selected);
        HashSet<ArmorComponent.ArmorMaterialTypes> _selected = new();
        public IReadOnlyCollection<ArmorComponent.ArmorMaterialTypes> Selected => _selected;

        protected override List<InquiryElement> BuildElements()
        {
            return Map.Keys.Select(k => new InquiryElement(k, k, null, true, "")).ToList();
        }

        protected override void ApplySelection(List<InquiryElement> selected)
        {
            if (selected == null) return;
            List<ArmorComponent.ArmorMaterialTypes> values = new();
            foreach (InquiryElement element in selected)
                if (element.Identifier is string s && Map.ContainsKey(s)) values.Add(Map[s]);
            _selected = new HashSet<ArmorComponent.ArmorMaterialTypes>(values);
        }

        protected override void OnReset()
        {
            _selected = new HashSet<ArmorComponent.ArmorMaterialTypes>();
        }
    }
}
