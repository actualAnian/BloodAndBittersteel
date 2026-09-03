using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops
{
    public static class BasicCharacterObjectExtensions
    {
        public static bool IsDynamicCharacter(this BasicCharacterObject character)
        {
            return DynamicTroopsService.Instance.IsDynamic(character.StringId);
        }
    }
}
