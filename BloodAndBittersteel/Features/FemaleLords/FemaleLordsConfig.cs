using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace BloodAndBittersteel.Features.FemaleLords
{
    public static class FemaleLordsConfig
    {
        public static bool CanLeadParties(CharacterObject characterObject)
        {
            return !characterObject.IsFemale
            || !BaBSettings.Instance.FemalePrejudice 
            || characterObject.Culture.StringId == Globals.WildlingsCultureId
            || characterObject.GetTraitLevel(DefaultTraits.Valor) >= 1;
        }
    }
}
