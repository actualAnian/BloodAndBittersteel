using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace LanceSystem.CampaignBehaviors
{
    public class NotablesInCastlesBehavior : RecruitmentCampaignBehavior
    {
        public static readonly Occupation occupation = Occupation.Headman;
        public int NotableCountInCastles { get { return 2; } }
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(DailyTickSettlement));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnNewGameCreated));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, WeeklyTick);
            CampaignEvents.CanHaveCampaignIssuesEvent.AddNonSerializedListener(this, CanHaveEvents);
        }

        private void CanHaveEvents(Hero hero, ref bool result)
        {
            result = false;
        }

        private void DailyTickSettlement(Settlement settlement)
        {
            if (settlement.IsCastle)
                DailyNotablePower(settlement);
        }

        private void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
        {
            SpawnNotablesAtGameStart();
        }
        private void DailyNotablePower(Settlement settlement)
        {
            foreach (Hero notable in settlement.Notables)
            {
                bool isStarving = settlement.IsStarving;
                if (isStarving)
                    notable.AddPower(-1f);
                else
                    notable.AddPower(settlement.Town.Prosperity / 10000f - 0.2f);
            }
        }
        private void WeeklyTick()
        {
            foreach (Settlement castle in Settlement.All.WhereQ((s) => s.IsCastle))
                SpawnNotablesIfNeeded(castle);
        }
        public void SpawnNotablesIfNeeded(Settlement settlement)
        {
            float countToSpawn = settlement.Notables.Any() ? (NotableCountInCastles - settlement.Notables.Count) / (float)NotableCountInCastles : 1f;
            countToSpawn *= (float)Math.Pow((double)countToSpawn, 0.36000001430511475);
            if (MBRandom.RandomFloat <= countToSpawn)
                EnterSettlementAction.ApplyForCharacterOnly(HeroCreator.CreateNotable(occupation, settlement), settlement);
        }
        private void SpawnNotablesAtGameStart()
        {
            foreach (Settlement settlement in Settlement.All)
            {
                if (settlement != null && settlement.IsCastle)
                {
                    int targetNotableCountForSettlement = NotableCountInCastles;
                    for (int i = 0; i < targetNotableCountForSettlement; i++)
                        HeroCreator.CreateNotable(occupation, settlement);
                }
            }
        }
    }

}