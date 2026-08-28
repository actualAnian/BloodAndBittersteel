using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CharacterFilters
{
    public class CharacterNameFilter : IDataFilter<CharacterObject>
    {
        readonly CharacterNameContext _context;
        readonly Func<CharacterObject, string> _nameSelector;
        public CharacterNameFilter(CharacterNameContext context, Func<CharacterObject, string> nameSelector)
        {
            _context = context;
            _nameSelector = nameSelector;
        }
        public IList<CharacterObject> GetFilteredItems(IList<CharacterObject> data)
        {
            if (string.IsNullOrWhiteSpace(_context.Query)) return data;
            return data.Where(c => (_nameSelector(c) ?? "").IndexOf(_context.Query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
    }
}
