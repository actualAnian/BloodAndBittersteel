using TaleWorlds.Core;

namespace LanceSystem.DynamicTroops
{
    public static class BasicCharacterObjectExtensions
    {
        public static bool IsDynamicCharacter(this BasicCharacterObject character)
        {
            if (character == null)
                return false;
            return DynamicTroopsService.Instance.IsDynamic(character.StringId);
        }
    }
}
