using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace LanceSystem
{
    public static class PartyBaseLancesExtensions
    {
        static LancesCampaignBehavior? lanceBehavior;
        public static List<LanceData> Lances(this PartyBase party)
        {
            lanceBehavior = Campaign.Current.GetCampaignBehavior<LancesCampaignBehavior>();
            return lanceBehavior.GetOrCreateLances(party);
        }
    }
}
