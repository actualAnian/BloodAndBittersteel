using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.KingsGuard
{
    public static class KingsguardConfig
    {
        public static readonly List<string> KingsguardMembersAtGameStart = new()
        {
            "TARG_m_13",
            "TARG_m_17",
            "TARG_m_16",
            "TARG_m_14",
            "TARG_m_15"
        };
        public static bool BelongsToKingsguard(this Hero hero)
        {
            return KingsguardMembersAtGameStart.Contains(hero.StringId);
        }
    }
}
