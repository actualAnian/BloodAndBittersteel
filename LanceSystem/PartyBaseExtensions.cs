using LanceSystem.CampaignBehaviors;
using LanceSystem.LanceDataClasses;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        public static bool HasFreeLanceSlots(this PartyBase party)
        {
            return Campaign.Current.Models.LanceModel().MaxLancesForParty(party).RoundedResultNumber < party.Lances().Count;
        }
    }
}
