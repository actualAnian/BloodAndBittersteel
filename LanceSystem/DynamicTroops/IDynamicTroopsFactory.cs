using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops
{
    public interface IDynamicTroopsFactory
    {
        CharacterObject CreateCharacterFromData(TroopCreationDTO data);
        void UpdateCharacterFromData(TroopCreationDTO data);
    }    
}
