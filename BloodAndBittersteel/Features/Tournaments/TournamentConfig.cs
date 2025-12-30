using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace BloodAndBittersteel.Features.Tournaments
{
    public static class TournamentConfig
    {
        public static bool CanParticipate(CharacterObject characterObject)
        {
            return !characterObject.IsFemale || characterObject.GetTraitLevel(DefaultTraits.Valor) >= 1;
        }
    }
}