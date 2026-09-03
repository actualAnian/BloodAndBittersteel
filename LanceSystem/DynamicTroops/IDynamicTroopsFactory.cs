using TaleWorlds.CampaignSystem;

namespace LanceSystem.DynamicTroops
{
    public interface IDynamicTroopsFactory
    {
        CharacterObject CreateCharacterFromData(TroopEditorData data);
        void UpdateCharacterFromData(TroopEditorData data);
    }    
}
