using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace BloodAndBittersteel.Features.Jousting
{
    internal class BaBTournamentModel : TournamentModel
    {
        private TournamentModel _baseModel;

        public BaBTournamentModel(TournamentModel baseModel)
        {
            _baseModel = baseModel;
        }

        public override TournamentGame CreateTournament(Town town)
        {
            return new JoustTournamentGame(town);
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
            var eq = new Equipment();
            var cataphract = MBObjectManager.Instance.GetObject<CharacterObject>("imperial_cataphract");
            return cataphract.Equipment;
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
