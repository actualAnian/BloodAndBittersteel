using System.Collections.Generic;
using System.Linq;
using LanceSystem.UI;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.ArmourMaterialFilters
{
    public class ArmourMaterialContext : MultiSelectionContext
    {
        static readonly Dictionary<ArmorComponent.ArmorMaterialTypes, TextObject> DisplayNames = new()
        {
            { (ArmorComponent.ArmorMaterialTypes)3, new("{=lance_material_chainmail}Chainmail") },
            { (ArmorComponent.ArmorMaterialTypes)1, new("{=lance_material_cloth}Cloth") },
            { (ArmorComponent.ArmorMaterialTypes)2, new("{=lance_material_leather}Leather") },
            { (ArmorComponent.ArmorMaterialTypes)4, new("{=lance_material_plate}Plate") }
        };
        static readonly Dictionary<string, ArmorComponent.ArmorMaterialTypes> Map = new()
        {
            { "Chainmail", (ArmorComponent.ArmorMaterialTypes)3 },
            { "Cloth", (ArmorComponent.ArmorMaterialTypes)1 },
            { "Leather", (ArmorComponent.ArmorMaterialTypes)2 },
            { "Plate", (ArmorComponent.ArmorMaterialTypes)4 }
        };

        public override string Title => new TextObject("{=lance_armour_material}Armour Material").ToString();
        public override string DisplayText => _selected.Count == 0 ? UITexts.All.ToString() : string.Join(", ", _selected.Select(s => DisplayNames[s].ToString()));
        HashSet<ArmorComponent.ArmorMaterialTypes> _selected = new();
        public IReadOnlyCollection<ArmorComponent.ArmorMaterialTypes> Selected => _selected;

        protected override List<InquiryElement> BuildElements()
        {
            return Map.Select(kv => new InquiryElement(kv.Key, DisplayNames[kv.Value].ToString(), null, true, "")).ToList();
        }

        protected override void ApplySelection(List<InquiryElement> selected)
        {
            if (selected == null) return;
            List<ArmorComponent.ArmorMaterialTypes> values = new();
            foreach (InquiryElement element in selected)
                if (element.Identifier is string s && Map.ContainsKey(s)) values.Add(Map[s]);
            _selected = new HashSet<ArmorComponent.ArmorMaterialTypes>(values);
        }

        protected override void OnResetInternal()
        {
            _selected = new HashSet<ArmorComponent.ArmorMaterialTypes>();
        }
    }
}
