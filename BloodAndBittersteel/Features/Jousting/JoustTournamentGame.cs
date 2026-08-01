using BloodAndBittersteel.Features.Jousting.JoustingMission;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.Jousting
{
    public class JoustTournamentGame : FightTournamentGame
    {
        public override int MaxTeamSize => 1;
        public override int MaxTeamNumberPerMatch => 2;

        public JoustTournamentGame(Town town) : base(town)
        {
            Mode = QualificationMode.IndividualScore;
        }

        public override void OpenMission(Settlement settlement, bool isPlayerParticipating)
        {
            JoustingMissionManager.OpenJoustingFightMission("Riverlands_Tournament_Arena", this, settlement, settlement.Culture, isPlayerParticipating);
        }
        protected override ItemObject GetTournamentPrize(bool includePlayer, int lastRecordedLordCountForTournamentPrize)
        {
            return JoustingConfig.GetJoustingRewardItem.Get(Town);
        }
    }
}
