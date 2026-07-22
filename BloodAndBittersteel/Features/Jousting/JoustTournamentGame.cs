using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
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

        public override bool CanBeAParticipant(CharacterObject character, bool considerSkills)
        {
            //if (character.Race == FaceGen.GetRaceOrDefault("large_humanoid_monster") ||
            //    character.Race == FaceGen.GetRaceOrDefault("medium_humanoid_monster") ||
            //    //character.HasAttribute("HasAnimationTriggeredEffects") ||
            //    character.Culture?.StringId == "chaos_culture")
            //{
            //    return false;
            //}
                
            return base.CanBeAParticipant(character, considerSkills);
        }

        public override void OpenMission(Settlement settlement, bool isPlayerParticipating)
        {
            int upgradeLevel = settlement.IsTown ? settlement.Town.GetWallLevel() : 1;
            JoustingMissionManager.OpenJoustingFightMission("Riverlands_Tournament_Arena", this, settlement, settlement.Culture, isPlayerParticipating);
        }
    }
}
