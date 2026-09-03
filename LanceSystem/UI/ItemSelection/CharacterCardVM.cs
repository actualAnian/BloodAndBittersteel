using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Localization;

namespace LanceSystem.UI.ItemSelection
{
    public class CharacterCardVM : CardVM
    {
        static readonly TextObject _unknownText = new("{=lance_unknown}Unknown");
        public CharacterCardVM(CharacterObject character, Action<CharacterObject> apply, Action? close = null)
            : base(() => apply(character), close, new CharacterImageIdentifierVM(CharacterCode.CreateFrom(character)))
        {
            CardName = character.Name?.ToString() ?? _unknownText.ToString();
            AddCharacterProperties(character);
        }

        void AddCharacterProperties(CharacterObject character)
        {
            ObjectProperties.Add(new ItemMenuTooltipPropertyVM(UITexts.TierLabel.ToString(), character.Tier.ToString(), 0, false, null));
            if (character.Culture?.Name != null)
                ObjectProperties.Add(new ItemMenuTooltipPropertyVM(UITexts.CultureLabel.ToString(), character.Culture.Name.ToString(), 0, false, null));
            var genderText = character.IsFemale ? UITexts.Female : UITexts.Male;
            ObjectProperties.Add(new ItemMenuTooltipPropertyVM(UITexts.GenderLabel.ToString(), genderText.ToString(), 0, false, null));
        }
    }
}
