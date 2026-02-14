using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodAndBittersteel.Features.CampaignCheats
{
    internal sealed class CampaignCheatsGlobalConfig
    {
        private static CampaignCheatsGlobalConfig? _instance;

        public static CampaignCheatsGlobalConfig Instance => _instance ??= new CampaignCheatsGlobalConfig();
        private CampaignCheatsGlobalConfig() { }
        public bool RebellionsEnabled { get; set; } = false;
    }
}
