using BloodAndBittersteel.Features.Jousting;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;

namespace BloodAndBittersteel.Features.Tournaments
{
    public class BaBFightTournamentGame : FightTournamentGame
    {
        public BaBFightTournamentGame(Town town) : base(town) {}
        protected override ItemObject GetTournamentPrize(bool includePlayer, int lastRecordedLordCountForTournamentPrize)
        {
            return JoustingConfig.GetJoustingRewardItem.Get(Town);
        }
    }
}
