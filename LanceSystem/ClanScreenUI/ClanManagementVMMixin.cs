using System.Collections.Generic;
using System.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using LanceSystem.DynamicLances;
using LanceSystem.DynamicTroops;
using LanceSystem.Extensions;
using LanceSystem.UI;
using LanceSystem.UI.LanceEditor;
using LanceSystem.UI.TroopEditor;
using LanceSystem.UI.TroopEditor.Services;
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
        static readonly TextObject _noTroopsAvailableText = new("{=lance_no_troops_available}No troops available for editing");
        static readonly TextObject _selectTroopHint = new("{=lance_select_troop_hint}Choose troop to edit");
        static readonly TextObject _createNewTroopText = new("{=lance_create_new_troop}Create New Troop");
        static readonly TextObject _createNewTemplateText = new("{=lance_create_new_template}Create New Template");
        static readonly TextObject _selectLanceTemplateTitle = new("{=lance_select_template}Select Lance Template");
        static readonly TextObject _selectLanceTemplateHint = new("{=lance_select_template_hint}Choose template to edit");

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
                InformationManager.DisplayMessage(new InformationMessage(_noTroopsAvailableText.ToString()));
                return;
            }
            elements.Sort(static (a, b) => string.Compare(a.Title, b.Title));
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(UITexts.SelectTroop.ToString(), _selectTroopHint.ToString(), elements, true, 1, 1, UITexts.Open.ToString(), null, args =>
            {

                if (args == null || !args.Any()) return;
                InformationManager.HideInquiry();
                string id = args.First().Identifier.ToString();
                CharacterObject? character = default;
                if (id == "create_new_troop")
                {
                    var newTroop = CharacterObjectExtension.CreateFromWithoutAddingToManager(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit"));
                    var setName = AccessTools.Method("TaleWorlds.Core.BasicCharacterObject:SetName");
                    setName.Invoke(newTroop, new object[] { _createNewTroopText });
                    character = newTroop;
                }
                else character = MBObjectManager.Instance.GetObject<CharacterObject>(id);
                if (character == null)
                {
                    InformationManager.DisplayMessage(new("Error, character picked is null, can not open troop editor"));
                    return;
                }
                var controller = new TroopEditorController(character);
                TroopEditorViewService.Create(controller.Vm);
            }, null, "", false), false, false);
        }

        List<InquiryElement> BuildTroopElements()
        {
            List<InquiryElement> elements = new()
            {
                CreateNewTroopElement()
            };
            foreach (string id in DynamicTroopsService.Instance.DynamicIds)
            {
                var co = MBObjectManager.Instance.GetObject<CharacterObject>(id);
                elements.Add(CreateTroopElement(co));
            }
            return elements;
        }

        InquiryElement CreateTroopElement(CharacterObject co)
        {
            return new InquiryElement(co.StringId, ((object)co.Name).ToString(), new TaleWorlds.Core.ImageIdentifiers.CharacterImageIdentifier(CharacterCode.CreateFrom(co)));
        }
        InquiryElement CreateNewTroopElement()
        {
            var tempTroop = MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit");
            return new InquiryElement("create_new_troop", _createNewTroopText.ToString(), new TaleWorlds.Core.ImageIdentifiers.CharacterImageIdentifier(CharacterCode.CreateFrom(tempTroop)));
        }

        void OpenLanceEditorWithPicker()
        {
            var lances = DynamicLancesService.Instance.GetAllLances();
            List<InquiryElement> elements = lances.Select(l => new InquiryElement(l.StringId, l.Name, null)).OrderBy(e => e.Title).ToList();
            elements.Insert(0, new InquiryElement("__new__", _createNewTemplateText.ToString(), null));
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_selectLanceTemplateTitle.ToString(), _selectLanceTemplateHint.ToString(), elements, true, 1, 1, UITexts.Open.ToString(), null, args =>
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
