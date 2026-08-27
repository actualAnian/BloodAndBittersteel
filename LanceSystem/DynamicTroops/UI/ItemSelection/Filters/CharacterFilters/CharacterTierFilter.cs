using LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops.TroopCreation.ItemSelection.Filters.CharacterFilters
{
    public class CharacterTierFilter : IDataFilter<CharacterObject>
    {
        readonly CharacterTierContext _context;
        public CharacterTierFilter(CharacterTierContext context)
        {
            _context = context;
        }
        public IList<CharacterObject> Filter(IList<CharacterObject> data)
        {
            if (_context.Selected.Count == 7) return data;
            return data.Where(c => c != null && _context.Selected.Contains((int)c.Tier)).ToList();
        }
    }
}
