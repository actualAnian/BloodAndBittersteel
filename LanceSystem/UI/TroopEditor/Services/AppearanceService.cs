using LanceSystem.UI.ItemSelection;
using LanceSystem.UI.ItemSelection.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.UI.TroopEditor.Services
{
    public class AppearanceService
    {
        static readonly TextObject _renameTitle = new("{=lance_rename_title}Rename");
        static readonly TextObject _proceedText = new("{=lance_proceed}Proceed");
        static readonly TextObject _selectCultureTitle = new("{=lance_select_culture}Select Culture");
        static readonly TextObject _selectFaceTitle = new("{=lance_select_face}Select Face");
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
            InformationManager.ShowTextInquiry(new TextInquiryData(_renameTitle.ToString(), UITexts.RenameHint.ToString(), true, true, _proceedText.ToString(), GameTexts.FindText("str_cancel").ToString(), text => SetName(text), null, false, null, "", ""), false, false);
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
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_selectCultureTitle.ToString(), "", elements, true, 1, 1, GameTexts.FindText("str_continue", null).ToString(), null, args =>
            {
                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                var culture = args.Select(e => e.Identifier as CultureObject).First();
                if (culture != null) _culture = culture;
                _refresh();
            }, null, "", false), false, false);
        }
        public void OpenAppearanceSelector()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier || c.Occupation == Occupation.Lord)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>(_selectFaceTitle.ToString(), allCharacters,
                (selectedCharacter) => { _bodyProperty = selectedCharacter.BodyPropertyRange; _refresh(); },
                (character, close) => new CharacterCardVM(character, (selectedCharacter) => { _bodyProperty = selectedCharacter.BodyPropertyRange; _refresh(); },
                close), FilterFactory.CreateCharacterFiltersWithOccupation());
            controller.Open();
        }
        public void ToggleGender()
        {
            _isFemale = !_isFemale;
            _refresh();
        }
    }
}
