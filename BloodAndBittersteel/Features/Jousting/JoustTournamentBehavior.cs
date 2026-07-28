using SandBox.Tournaments;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace BloodAndBittersteel.Features.Jousting
{
    public class JoustTournamentBehavior: TournamentBehavior
    {
        public JoustTournamentBehavior(TournamentGame tournamentGame, Settlement settlement, ITournamentGameBehavior gameBehavior, bool isPlayerParticipating) : base(tournamentGame, settlement, gameBehavior, isPlayerParticipating) { }
    }
}
