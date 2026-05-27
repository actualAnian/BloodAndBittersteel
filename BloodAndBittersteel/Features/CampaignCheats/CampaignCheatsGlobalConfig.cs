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
