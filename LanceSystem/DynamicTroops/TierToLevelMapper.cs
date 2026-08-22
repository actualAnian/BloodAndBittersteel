using System.Collections.Generic;

namespace LanceSystem.DynamicTroops
{
    public static class TierToLevelMapper
    {
        public static int GetLevelForTier(int tier)
        {
            return tier switch
            {
                0 => 1,
                1 => 6,
                2 => 11,
                3 => 16,
                4 => 21,
                5 => 26,
                6 => 31,
                _ => 36,
            };
        }
    }
}
