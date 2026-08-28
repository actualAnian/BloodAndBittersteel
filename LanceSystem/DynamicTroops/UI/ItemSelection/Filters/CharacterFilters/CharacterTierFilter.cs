using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.UI.ItemSelection.Filters.CharacterFilters
{
    public class CharacterTierFilter : IDataFilter<CharacterObject>
    {
        readonly CharacterTierContext _context;
        public CharacterTierFilter(CharacterTierContext context)
        {
            _context = context;
        }
        public IList<CharacterObject> GetFilteredItems(IList<CharacterObject> data)
        {
            if (_context.Selected.Count == 7) return data;
            return data.Where(c => c != null && _context.Selected.Contains((int)c.Tier)).ToList();
        }
    }
}
