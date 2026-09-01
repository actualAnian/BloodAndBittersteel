using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;

namespace LanceSystem.DynamicTroops.UI.ItemSelection
{
    public class CharacterCardVM : CardVM
    {
        public CharacterCardVM(CharacterObject character, Action<CharacterObject> apply, Action? close = null)
            : base(() => apply(character), close)
        {
            CardName = character.Name?.ToString() ?? "Unknown";
            Image = new CharacterImageIdentifierVM(CharacterCode.CreateFrom(character));
            AddCharacterProperties(character);
        }

        void AddCharacterProperties(CharacterObject character)
        {
            ObjectProperties.Add(new ItemMenuTooltipPropertyVM("Tier: ", (character.Tier + 1).ToString(), 0, false, null));
            if (character.Culture?.Name != null)
                ObjectProperties.Add(new ItemMenuTooltipPropertyVM("Culture: ", character.Culture.Name.ToString(), 0, false, null));
            string gender = character.IsFemale ? "Female" : "Male";
            ObjectProperties.Add(new ItemMenuTooltipPropertyVM("Gender: ", gender, 0, false, null));
        }
    }
}
