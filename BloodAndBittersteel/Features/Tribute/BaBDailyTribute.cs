using BloodAndBittersteel.Features.BlackfyreRebellion;
using BloodAndBittersteel.Features.ModifiableValues;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace BloodAndBittersteel.Features
{
    internal class BaBDailyTribute : CampaignBehaviorBase
    {
        const float BaseModifier = 0.1f;
        TimedModifierNumber TributeAmountModifier => _tributeAmountModifier;
        TimedModifierNumber _tributeAmountModifier;

        public BaBDailyTribute()
        {
            _tributeAmountModifier = new(BaseModifier);
        }
        public override void RegisterEvents()
        {
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, SetTributeToPayWeekly);
        }

        private void SetTributeToPayWeekly()
        {
            TributeAmountModifier.AddModifier(new("test", 0.1f, CampaignTime.Never));
            var rebellionBehavior = Campaign.Current.GetCampaignBehavior<RebellionCampaignBehavior>();
            var vassalKingdoms = Campaign.Current.Kingdoms.Where(k => !k.IsEliminated && rebellionBehavior.RebellionData.LoyalistVassals.Contains(k.StringId));
            var mainKingdoms = Campaign.Current.Kingdoms.Where(k => !k.IsEliminated && RebellionConfig.LoyalistFactions.Contains(k.StringId));

            foreach (var kingdom in vassalKingdoms)
            {
                ExplainedNumber KingdomIncome = new();
                foreach (var clan in kingdom.Clans)
                {
                    foreach (var town in clan.Fiefs)
                    {
                        KingdomIncome.Add(Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(town, false).ResultNumber);
                        KingdomIncome.Add(Campaign.Current.Models.ClanFinanceModel.CalculateTownIncomeFromTariffs(clan, town, false).ResultNumber);
                        KingdomIncome.Add(Campaign.Current.Models.ClanFinanceModel.CalculateTownIncomeFromProjects(town));
                        foreach (var village in town.Villages)
                        {
                            KingdomIncome.Add(Campaign.Current.Models.ClanFinanceModel.CalculateVillageIncome(clan, village, false));
                        }
                    }
                }
                var days = Campaign.Current.Models.CampaignTimeModel.DaysInWeek;
                var totalStrength = mainKingdoms.Sum(k => k.CurrentTotalStrength);
                var tributeToPay = KingdomIncome.ResultNumber * TributeAmountModifier.CurrentValue * days;

                foreach (var mainKingdom in mainKingdoms)
                {
                    var strengthRatio = mainKingdom.CurrentTotalStrength / totalStrength;
                    var link = kingdom.GetStanceWith(mainKingdom);
                    link.SetDailyTributePaid(kingdom, (int)(tributeToPay * strengthRatio / days), days);
                }
            }
        }
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("bab_tributeamountmodifier", ref _tributeAmountModifier);
        }
    }
}