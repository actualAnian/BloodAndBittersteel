using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using static BloodAndBittersteel.Features.LanceSystem.LancesCampaignBehavior;

namespace BloodAndBittersteel.Features.LanceSystem
{
    internal class BaBLanceModel : LanceModel
    {
        public override int LancesFromClanTier(int clanTier)
        {
            return clanTier;
        }

        public override ExplainedNumber MaxLancesForParty(PartyBase party)
        {
            var number = new ExplainedNumber();
            number.Add(LancesFromClanTier(party.Owner.Clan.Tier), new("{bab_lances_from_clan}Lances from clan tier"));
            return number;
        }

        public override int MaxNotableTroops(Hero notable)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateNotablesLanceTroops(Hero notable, NotableLanceData lanceData)
        {
            var availableLanceTroops = lanceData.CurrentNotableLanceRoster;
            if (availableLanceTroops.Count < 10)
            {
                availableLanceTroops.AddToCounts(notable.Culture.EliteBasicTroop, 1);
            }
        }
    }
}
