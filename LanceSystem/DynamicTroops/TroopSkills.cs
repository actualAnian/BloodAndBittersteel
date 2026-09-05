using System.Collections.Generic;
using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops
{
    public static class TroopSkills
    {
        public static readonly IReadOnlyList<SkillObject> All = new List<SkillObject>
        {
            DefaultSkills.OneHanded,
            DefaultSkills.TwoHanded,
            DefaultSkills.Polearm,
            DefaultSkills.Bow,
            DefaultSkills.Crossbow,
            DefaultSkills.Throwing,
            DefaultSkills.Athletics,
            DefaultSkills.Riding
        };
    }
}
