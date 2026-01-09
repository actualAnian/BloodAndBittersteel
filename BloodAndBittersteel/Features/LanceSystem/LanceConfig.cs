using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BloodAndBittersteel.Features.LanceSystem
{
    public static class LanceConfig
    {
        public static TextObject GetLanceName(Hero notable, Settlement settlement)
        {
            switch (notable.Culture.StringId)
            {
                default:
                    TextObject template = new("{=bab_lance_name_default}Lance of {NOTABLE_NAME} from {SETTLEMENT_NAME");
                    GameTexts.SetVariable("NOTABLE_NAME", notable.Name);
                    GameTexts.SetVariable("SETTLEMENT_NAME", settlement.Name);
                    return template;
            }
        }
    }
}