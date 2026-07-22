using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;

namespace BloodAndBittersteel.Features.Jousting
{
    public class JoustingCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents() 
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public void OnSessionLaunched(CampaignGameStarter starter)
        {
            AddGameMenus(starter);
            //RegisterCustomTournamentType(starter);
        }

        private void AddGameMenus(CampaignGameStarter starter)
        {
            // Insert "Joust Tournament" option into town_arena menu (between Join and Watch)
            starter.AddGameMenuOption("town_arena", "join_joust_tournament",
                "{=your_string_id}Enter the jousting tournament",
                CanJoinJoustOnCondition, EnterJoustTournamentConsequence, isLeave: false, index: 1);
        }

        private bool CanJoinJoustOnCondition(MenuCallbackArgs args)
        {
            return true;
            //return Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(
            //    Settlement.CurrentSettlement,
            //    SettlementAccessModel.SettlementAction.JoinTournament,);
        }

        private void EnterJoustTournamentConsequence(MenuCallbackArgs args)
        {
            var town = Settlement.CurrentSettlement.Town;
            var joustGame = new JoustTournamentGame(town);  // Standalone JoustTournamentGame
            joustGame.OpenMission(Settlement.CurrentSettlement, isPlayerParticipating: true);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }

}
