using BloodAndBittersteel.Features.Tournaments;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BloodAndBittersteel.Models
{
    public class BaBTournamentModel : TournamentModel
    {
        public BaBTournamentModel(TournamentModel baseModel)
        {
            _baseModel = baseModel;
        }
        TournamentModel _baseModel;

        public override TournamentGame CreateTournament(Town town)
        {
            return new BaBFightTournamentGame(town);
        }

        public override MBList<ItemObject> GetEliteRewardItems(Town town, int regularRewardMinValue, int regularRewardMaxValue)
        {
            return _baseModel.GetEliteRewardItems(town, regularRewardMinValue, regularRewardMaxValue);
        }

        public override int GetInfluenceReward(Hero winner, Town town)
        {
            return _baseModel.GetInfluenceReward(winner, town);
        }

        public override int GetNumLeaderboardVictoriesAtGameStart()
        {
            return _baseModel.GetNumLeaderboardVictoriesAtGameStart();
        }

        public override Equipment GetParticipantArmor(CharacterObject participant)
        {
            return _baseModel.GetParticipantArmor(participant);
        }

        public override MBList<ItemObject> GetRegularRewardItems(Town town, int regularRewardMinValue, int regularRewardMaxValue)
        {
            return _baseModel.GetRegularRewardItems(town, regularRewardMinValue, regularRewardMaxValue);
        }

        public override int GetRenownReward(Hero winner, Town town)
        {
            return _baseModel.GetRenownReward(winner, town);
        }

        public override (SkillObject skill, int xp) GetSkillXpGainFromTournament(Town town)
        {
            return _baseModel.GetSkillXpGainFromTournament(town);
        }

        public override float GetTournamentEndChance(TournamentGame tournament)
        {
            return _baseModel.GetTournamentEndChance(tournament);
        }

        public override float GetTournamentSimulationScore(CharacterObject character)
        {
            return _baseModel.GetTournamentSimulationScore(character);
        }

        public override float GetTournamentStartChance(Town town)
        {
            return _baseModel.GetTournamentStartChance(town);
        }
    }
}
