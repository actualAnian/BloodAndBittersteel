using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Features.CampaignCheats
{
    internal class EnableRebellionsCheat
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("are_rebellions_enabled", "bab")]
        public static string AreRebellionsEnabledCheat(List<string> strings)
        {
            if (strings.Count != 1)
                return "Invalid number of arguments. Usage: enable_rebellions 1/0.";

            if (!int.TryParse(strings[0], out int result))
                return "Invalid argument. Please specify 1 to enable or 0 to disable.";

            if (result == 1)
            {
                CampaignCheatsGlobalConfig.Instance.RebellionsEnabled = true;
                return "Rebellions enabled.";
            }
            else if (result == 0)
            {
                CampaignCheatsGlobalConfig.Instance.RebellionsEnabled = false;
                return "Rebellions disabled.";
            }
            return "Invalid value. Use 1 to enable or 0 to disable.";
        }
    }
}
