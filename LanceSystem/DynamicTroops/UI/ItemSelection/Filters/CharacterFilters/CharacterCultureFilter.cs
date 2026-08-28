using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CharacterFilters
{
    public class CharacterCultureFilter : IDataFilter<CharacterObject>
    {
        readonly CharacterCultureContext _context;
        public CharacterCultureFilter(CharacterCultureContext context)
        {
            _context = context;
        }
        public IList<CharacterObject> GetFilteredItems(IList<CharacterObject> data)
        {
            if (_context.Selected.Count == 0) return data;
            return data.Where(c => c?.Culture != null && _context.Selected.Contains(c.Culture)).ToList();
        }
    }
}
