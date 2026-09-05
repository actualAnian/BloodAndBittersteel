using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace LanceSystem.UI.ItemSelection.Filters.CharacterTypeFilters
{
    public class CharacterTypeContext : MultiSelectionContext
    {
        public override string Title => new TextObject("{=lance_character_type}Character Type").ToString();
        public override string DisplayText => _selected.Count == 2 ? UITexts.All.ToString() : string.Join(", ", _selected);
        HashSet<Occupation> _selected = new() { Occupation.Soldier, Occupation.Lord };
        public static readonly List<Tuple<Occupation, TextObject>> Elements = new() { new(Occupation.Soldier, UITexts.Soldier), new(Occupation.Lord, UITexts.Lord) };
        public HashSet<Occupation> Selected => _selected;

        protected override List<InquiryElement> BuildElements()
        {
            List<InquiryElement> elements = new();
            foreach (var el in Elements)
                elements.Add(new InquiryElement(el.Item1, el.Item2.ToString(), null, true, ""));
            return elements;
        }

        protected override void ApplySelection(List<InquiryElement> selected)
        {
            List<Occupation> values = new();
            foreach (InquiryElement element in selected)
                values.Add((Occupation)element.Identifier);
            _selected = new HashSet<Occupation>(values);
        }

        protected override void OnResetInternal()
        {
            _selected = new HashSet<Occupation> { Occupation.Soldier, Occupation.Lord };
        }
    }
}
