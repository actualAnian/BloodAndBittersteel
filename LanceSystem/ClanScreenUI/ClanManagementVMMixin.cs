using System.Collections.Generic;
using System.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using LanceSystem.Deserialization;
using LanceSystem.DynamicLances.UI;
using LanceSystem.DynamicTroops;
using LanceSystem.DynamicTroops.UI;
using LanceSystem.DynamicTroops.UI.Services;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.ClanScreenUI
{
    [ViewModelMixin("RefreshValues", true)]
    public class ClanManagementVMMixin : BaseViewModelMixin<ClanManagementVM>
    {
        readonly ClanManagementVM _vm;

        public ClanManagementVMMixin(ClanManagementVM vm) : base(vm)
        {
            _vm = vm;
        }

        public override void OnRefresh() { }

        [DataSourceMethod]
        public void ExecuteOpenTroopEditor()
        {
            OpenTroopEditorWithPicker();
        }

        [DataSourceMethod]
        public void ExecuteOpenLanceTemplateEditor()
        {
            OpenLanceEditorWithPicker();
        }

        void OpenTroopEditorWithPicker()
        {
            List<InquiryElement> elements = BuildTroopElements();
            if (elements.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage("No troops available for editing"));
                return;
            }
            elements.Sort(static (a, b) => string.Compare(a.Title, b.Title));
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Troop", "Choose troop to edit", elements, true, 1, 1, "Open", null, args =>
            {
                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                string id = args.First().Identifier.ToString();
                CharacterObject character = MBObjectManager.Instance.GetObject<CharacterObject>(id) ?? Game.Current.ObjectManager.GetObject<CharacterObject>(id);
                if (character == null) return;
                TroopEditorController manager = new TroopEditorController(character);
                TroopEditorViewService.Create(manager.Vm);
            }, null, "", false), false, false);
        }

        List<InquiryElement> BuildTroopElements()
        {
            List<InquiryElement> elements = new();
            foreach (string id in DynamicTroopsService.Instance.DynamicIds)
            {
                var co = MBObjectManager.Instance.GetObject<CharacterObject>(id);
                elements.Add(CreateTroopElement(co));
            }
            if (elements.Count != 0) return elements;

            var newTroop = CharacterObject.CreateFrom(MBObjectManager.Instance.GetObject<CharacterObject>("looter"));
            var setName = AccessTools.Method("TaleWorlds.Core.BasicCharacterObject:SetName");
            setName.Invoke(newTroop, new object[] { new TextObject("New Troop") });

            return elements;
        }

        InquiryElement CreateTroopElement(CharacterObject co)
        {
            return new InquiryElement(co.StringId, ((object)co.Name).ToString(), new TaleWorlds.Core.ImageIdentifiers.CharacterImageIdentifier(CharacterCode.CreateFrom(co)));
        }

        void OpenLanceEditorWithPicker()
        {
            var lances = LanceTemplateManager.Instance.Lances.Values.ToList();
            List<InquiryElement> elements = lances.Select(l => new InquiryElement(l.StringId, l.Name, null)).OrderBy(e => e.Title).ToList();
            elements.Insert(0, new InquiryElement("__new__", "Create New Template", null));
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Lance Template", "Choose template to edit", elements, true, 1, 1, "Open", null, args =>
            {
                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                string id = args.First().Identifier.ToString();
                if (id == "__new__") LanceTemplateEditorController.CreateLayer();
                else if (!string.IsNullOrWhiteSpace(id)) LanceTemplateEditorController.CreateLayer(id);
            }, null, "", false), false, false);
        }
    }
}
