using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
namespace LanceSystem.DynamicTroops.UI.Services
{
    public class AppearanceService
    {
        readonly CharacterObject _character;
        readonly System.Action _refresh;
        public AppearanceService(CharacterObject character, System.Action refresh)
        {
            _character = character;
            _refresh = refresh;
        }
        public void Rename()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData("Rename", "Enter new name", true, true, "Proceed", "Cancel", text => SetName(text), null, false, null, "", ""), false, false);
        }
        void SetName(string text)
        {
            typeof(BasicCharacterObject).GetMethod("SetName", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_character, new object[] { new TextObject(text) });
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
                SetCulture(culture);
            }, null, "", false), false, false);
        }
        void SetCulture(CultureObject culture)
        {
            typeof(BasicCharacterObject).GetProperty("Culture", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(_character, culture);
            UpdateAppearance();
            _refresh();
        }
        public void ChangeGender()
        {
            _character.IsFemale = !_character.IsFemale;
            UpdateAppearance();
            _refresh();
        }
        public void UpdateAppearance()
        {
            try
            {
                MBBodyProperty property = GetBodyProperty();
                typeof(CharacterObject).GetProperty("BodyPropertyRange")?.SetValue(_character, property, null);
            }
            catch { }
        }
        MBBodyProperty GetBodyProperty()
        {
            if ((_character.IsFemale ? _character.Culture.Townswoman : _character.Culture.Townsman) != null) return _character.IsFemale ? _character.Culture.FemaleDancer.BodyPropertyRange : _character.Culture.BasicTroop.BodyPropertyRange;
            return _character.IsFemale ? MBObjectManager.Instance.GetObject<CharacterObject>("female_dancer_empire").BodyPropertyRange : MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit").BodyPropertyRange;
        }
    }
}

