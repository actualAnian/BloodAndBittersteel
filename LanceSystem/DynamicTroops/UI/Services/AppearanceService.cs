using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops.UI.Services
{
    public class AppearanceService
    {
        readonly Action _refresh;
        string _name;
        bool _isFemale;
        CultureObject _culture;
        MBBodyProperty? _bodyProperty;
        public AppearanceService(CharacterObject character, Action refresh)
        {
            _refresh = refresh;
            _name = character.Name?.ToString() ?? "";
            _isFemale = character.IsFemale;
            _culture = character.Culture;
            _bodyProperty = character.BodyPropertyRange;
        }
        public string GetName() => _name;
        public bool GetIsFemale() => _isFemale;
        public CultureObject GetCulture() => _culture;
        public MBBodyProperty? GetBodyProperty() => _bodyProperty;
        public void Rename()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData("Rename", "Enter new name", true, true, "Proceed", "Cancel", text => SetName(text), null, false, null, "", ""), false, false);
        }
        void SetName(string text)
        {
            _name = text;
            _refresh();
        }
        public void ChangeCulture()
        {
            List<CultureObject> cultures = new();
            List<InquiryElement> elements = new();
            foreach (Kingdom kingdom in Campaign.Current.Kingdoms)
            {
                if (kingdom?.Culture == null || cultures.Contains(kingdom.Culture)) continue;
                cultures.Add(kingdom.Culture);
                elements.Add(new InquiryElement(kingdom.Culture, kingdom.Culture.Name.ToString(), new BannerImageIdentifier(kingdom.Banner)));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Culture", "", elements, true, 1, 1, "Continue", null, args =>
            {
                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                CultureObject culture = args.Select(e => e.Identifier as CultureObject).First();
                _culture = culture;
                _refresh();
            }, null, "", false), false, false);
        }
        public void OpenAppearanceSelector()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier || c.Occupation == Occupation.Lord)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>("Select Face", allCharacters,
                (selectedCharacter) => { _bodyProperty = selectedCharacter.BodyPropertyRange; _refresh(); },
                (character, close) => new CharacterCardVM(character, (selectedCharacter) => { _bodyProperty = selectedCharacter.BodyPropertyRange; _refresh(); },
                close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        public void ToggleGender()
        {
            _isFemale = !_isFemale;
            _refresh();
        }
    }
}
