using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features.CampaignStart
{
    public static class BaBCampaignStartConfig
    {
        public static string GetStartSettlementFromCulture(CultureObject playerCulture)
        {
            return "town_EN1";
        }
    }
}
