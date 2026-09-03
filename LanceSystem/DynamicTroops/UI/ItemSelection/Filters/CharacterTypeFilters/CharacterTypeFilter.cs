using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CharacterFilters
{
    public class CharacterTypeFilter : IDataFilter<CharacterObject>
    {
        readonly CharacterTypeContext _context;
        public CharacterTypeFilter(CharacterTypeContext context)
        {
            _context = context;
        }
        public IEnumerable<CharacterObject> GetFilteredItems(IEnumerable<CharacterObject> data)
        {
            if (_context.Selected.Count == CharacterTypeContext.Elements.Count) return data;
            return data.Where(c => c != null && _context.Selected.Contains(c.Occupation));
        }
    }
}
