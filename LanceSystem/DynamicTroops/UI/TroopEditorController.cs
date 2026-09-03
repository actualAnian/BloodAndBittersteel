using HarmonyLib;
using LanceSystem.Common;
using LanceSystem.Deserialization;
using LanceSystem.DynamicLances.UI;
using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;
using LanceSystem.DynamicTroops.UI.Services;
using LanceSystem.Extensions;
using LanceSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops.UI
{
    public class TroopEditorController
    {
        static readonly TextObject _formationPrefix = new("{=lance_formation_prefix}Formation");
        static readonly TextObject _formationInfantry = new("{=1Bm1Wk1v}Infantry");
        static readonly TextObject _formationRanged = new("{=bIiBytSB}Archers");
        static readonly TextObject _formationCavalry = new("{=YVGtcLHF}Cavalry");
        static readonly TextObject _formationHorseArcher = new("{=I1CMeL9R}Mounted Archers");
        public static readonly TextObject _tierLabel = new("{=cc1d7mkq}Tier");
        static readonly TextObject _setLabel = new("{=lance_set_label}Set");
        static readonly TextObject _selectSetsTitle = new("{=lance_select_sets_title}Select sets to remove");
        static readonly TextObject _savedPrefix = new("{=lance_saved_prefix}Saved");
        static readonly TextObject _copiedText = new("{=lance_copied}Troop template copied to clipboard");
        static readonly TextObject _noTemplateClipboardText = new("{=lance_no_template_clipboard}No troop template in clipboard. Copy a template first.");
        static readonly TextObject _unsavedPasteTitleText = new("{=lance_unsaved_paste_title}Unsaved Changes");
        static readonly TextObject _pasteSuccessText = new("{=lance_paste_success}Troop template pasted");

        static readonly TextObject _selectTierTitle = new("{=lance_select_tier_title}Select Tier");
        public CharacterObject PreviewCharacter { get; }
        public TroopEditorVM Vm { get; }
        readonly SkillService _skillService;
        readonly AppearanceService _appearanceService;
        readonly EquipmentService _equipmentService;
        readonly TroopUpgradeService _upgradeService;
        int _level;
        public TroopEditorController(CharacterObject character)
        {
            PreviewCharacter = CharacterObjectExtension.CreateFromWithoutAddingToManager(character);
            _level = character.Level;
            _skillService = new SkillService(character, () => GetTier(), Refresh);
            _appearanceService = new AppearanceService(character, Refresh);
            _equipmentService = new EquipmentService(character, Refresh);
            _upgradeService = new TroopUpgradeService(character, Refresh);
            Vm = new TroopEditorVM(PreviewCharacter, this);
        }
        public string GetName() => _appearanceService.GetName();
        public bool GetIsFemale() => _appearanceService.GetIsFemale();
        public CultureObject GetCulture() => _appearanceService.GetCulture();
        public MBBodyProperty? GetBodyProperty() => _appearanceService.GetBodyProperty();
        public List<Equipment> GetBattleEquipments() => _equipmentService.GetBattleEquipments();
        public FormationClass GetDefaultGroup() => _equipmentService.GetDefaultGroup();
        public MBEquipmentRoster GetRoster() => _equipmentService.BuildRoster();
        public List<CharacterObject> GetUpgradeTargets() => _upgradeService.GetTroopUpgradeTargets();
        public int GetLevel() => _level;
        public int GetTier()
        {
            if (_level <= 1) return 0;
            return Math.Max(0, (_level - 1) / 5);
        }
        public void SetTier(int tier)
        {
            _level = TierToLevelMapper.GetLevelForTier(tier);
        }
        public int GetSkillValue(SkillObject skill) => _skillService.GetSkillValue(skill);
        public int GetCurrentSkillSum() => _skillService.GetCurrentSkillSum();
        public int GetSuggestedMaxSkillSum() => _skillService.GetSuggestedMaxSkillSum();
        public void Refresh()
        {
            _equipmentService.RecalculateDefaultGroup();
            Vm.RefreshValues();
        }
        public void UpdateSkill(SkillObject skill, int amount) => _skillService.UpdateSkill(skill, amount);
        public void Rename() => _appearanceService.Rename();
        public void ChangeCulture() => _appearanceService.ChangeCulture();
        public void ChangeGender() => _appearanceService.ToggleGender();
        public void SelectAppearance() => _appearanceService.OpenAppearanceSelector();
        public void SelectUpgradeCharacter(CharacterObject previouscharacter)
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
            .Where(c => c.Occupation == Occupation.Soldier)
            .ToList();
            Action<CharacterObject> onSelect = (newCharacter =>
            {
                var slotToUpdate = Vm.UpgradeSlots.FirstOrDefault(slot => slot.Upgrade == previouscharacter);
                var slotWithNewCharacter = Vm.UpgradeSlots.Where(slot => slot.Upgrade == newCharacter);
                if (slotWithNewCharacter.Count() != 0 || slotToUpdate == null) return;
                slotToUpdate.ChangeCharacter(newCharacter);
            });
            var controller = new ObjectSelectorController<CharacterObject>(UITexts.SelectTroop.ToString(), allCharacters, onSelect, (character, close) => new CharacterCardVM(character, onSelect, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        public void SelectItem(string slotKey) => _equipmentService.SelectItem(slotKey);
        public void FinalizeItem(EquipmentIndex index, ItemObject item) => _equipmentService.FinalizeItem(index, item);
        public void SetDefaultGroup() => _equipmentService.RecalculateDefaultGroup();
        public void AddUpgrade()
        {
            var currentUpgrades = _upgradeService.GetTroopUpgradeTargets();
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier)
                .Where(c => !currentUpgrades.Contains(c))
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>(UITexts.SelectTroop.ToString(), allCharacters, _upgradeService.AddTroopUpgradeTarget, (character, close) => new CharacterCardVM(character, _upgradeService.AddTroopUpgradeTarget, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        public void RemoveUpgrade(CharacterObject upgrade) => _upgradeService.RemoveTroopUpgradeTarget(upgrade);
        public void CopyTemplate()
        {
            var preview = CreatePreviewCharacter();
            EditorTemplateHolder.Instance.TroopPreviewCharacter = preview;
            InformationManager.DisplayMessage(new InformationMessage(_copiedText.ToString()));
        }
        public void PasteTemplate()
        {
            var template = EditorTemplateHolder.Instance.TroopPreviewCharacter;
            if (template == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(_noTemplateClipboardText.ToString()));
                return;
            }
            if (Vm.HasUnsavedChanges)
            {
                InformationManager.ShowInquiry(new InquiryData(_unsavedPasteTitleText.ToString(), UITexts.UnsavedMessage.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), () => ApplyPaste(template), null, "", 0f, null, null, null), true, false);
                return;
            }
            ApplyPaste(template);
        }
        void ApplyPaste(CharacterObject template)
        {
            TroopEditorViewService.Delete();
            var controller = new TroopEditorController(template);
            TroopEditorViewService.Create(controller.Vm);
            InformationManager.DisplayMessage(new InformationMessage(_pasteSuccessText.ToString()));
        }

        public TroopEditorData BuildTroopEditorData()
        {
            return new TroopEditorData
            (
                GetName(),
                GetIsFemale(),
                GetDefaultGroup(),
                GetTier(),
                GetCulture(),
                GetUpgradeTargets(),
                GetRoster(),
                GetBodyProperty(),
                BuildSkillValues()
            );
        }
        public void OpenTierInquiry()
        {
            List<InquiryElement> list = new();
            for (int i = 1; i <= 6; i++)
            {
                MBTextManager.SetTextVariable("TIER", i);
                list.Add(new InquiryElement(i, new TextObject("{=lance_tier_inquiry}{TIER_LABEL} {TIER}").SetTextVariable("TIER_LABEL", _tierLabel).ToString(), null));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_selectTierTitle.ToString(), "", list, true, 1, 1, GameTexts.FindText("str_continue", null).ToString(), null, args =>
            {
                InformationManager.HideInquiry();
                SetTier((int)args.First().Identifier);
                Refresh();
            }, null, "", false), false, false);
        }
        public void AddSet()
        {
            _equipmentService.AddSet();
            Refresh();
        }
        public void RemoveSet()
        {
            List<Equipment> battle = _equipmentService.GetBattleEquipments();
            if (battle.Count <= 1) return;
            List<InquiryElement> elements = new();
            for (int i = 0; i < battle.Count; i++)
            {
                Equipment equipment = battle[i];
                string weapon = equipment[EquipmentIndex.Weapon0].Item?.Name?.ToString() ?? UITexts.Empty.ToString();
                MBTextManager.SetTextVariable("SET_NUM", i + 1);
                MBTextManager.SetTextVariable("WEAPON", weapon);
                elements.Add(new InquiryElement(i, new TextObject("{=lance_set_entry}{SET_LABEL} {SET_NUM} : {WEAPON}")
                    .SetTextVariable("SET_LABEL", _setLabel).ToString(), null));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(_selectSetsTitle.ToString(), "", elements, true, 1, battle.Count - 1, GameTexts.FindText("str_continue", null).ToString(), null, OnRemoveSetsConfirmed, null, "", false), false, false);
        }
        void OnRemoveSetsConfirmed(List<InquiryElement> args)
        {
            if (args == null || !args.Any()) return;
            InformationManager.HideInquiry();
            List<int> indicesToRemove = args.Select(e => (int)e.Identifier).OrderByDescending(i => i).ToList();
            _equipmentService.RemoveSets(indicesToRemove);
            Refresh();
        }
        public string FillFormationString()
        {
            var formationEnum = _equipmentService.GetDefaultGroup();
            var formationType = formationEnum switch
            {
                FormationClass.Infantry => _formationInfantry,
                FormationClass.Ranged => _formationRanged,
                FormationClass.Cavalry => _formationCavalry,
                FormationClass.HorseArcher => _formationHorseArcher,
                _ => _formationInfantry,
            };
            MBTextManager.SetTextVariable("FORMATION", formationType);
            return new TextObject("{=lance_formation_full}{FORMATION_PREFIX}: {FORMATION}")
                .SetTextVariable("FORMATION_PREFIX", _formationPrefix).ToString();
        }

        public MBBindingList<LanceBannerItemVM> GetLanceBannersForTroop(Action<string> onLanceClicked)
        {
            var banners = new MBBindingList<LanceBannerItemVM>();
            var lances = LanceTemplateManager.Instance.GetLancesForTroop(PreviewCharacter.StringId);
            foreach (var lance in lances)
                banners.Add(new LanceBannerItemVM(lance, onLanceClicked));
            return banners;
        }

        public void SwitchTroop()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>(new TextObject("{=lance_switch_troop_title}Switch Troop").ToString(), allCharacters, OnSwitchTroop, (character, close) => new CharacterCardVM(character, OnSwitchTroop, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        void OnSwitchTroop(CharacterObject selectedCharacter)
        {
            TroopEditorViewService.Delete();
            var newController = new TroopEditorController(selectedCharacter);
            TroopEditorViewService.Create(newController.Vm);
        }
        public void CreateNewTroop()
        {
            CharacterObject looter = MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit");
            CharacterObject newTroop = CharacterObjectExtension.CreateFromWithoutAddingToManager(looter);
            var setNameMethod = AccessTools.Method("TaleWorlds.Core.BasicCharacterObject:SetName");
            setNameMethod.Invoke(newTroop, new object[] { GetUniqueTroopName("NewTroop") });
            var controller = new TroopEditorController(newTroop);
            TroopEditorViewService.Delete();
            TroopEditorViewService.Create(controller.Vm);
        }
        TextObject GetUniqueTroopName(string baseName)
        {
            if (MBObjectManager.Instance.GetObject<CharacterObject>(baseName) == null) return new(baseName);
            for (int i = 1; ; i++)
            {
                string name = baseName + "_" + i;
                if (MBObjectManager.Instance.GetObject<CharacterObject>(name) == null) return new(name);
            }
        }
        public void OpenLanceTemplate(string lanceId)
        {
            if (Vm.HasUnsavedChanges)
                InformationManager.ShowInquiry(new InquiryData(UITexts.UnsavedTitle.ToString(), UITexts.UnsavedMessage.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), () => DoOpenLanceTemplate(lanceId), null, "", 0f, null, null, null), true, false);
            else DoOpenLanceTemplate(lanceId);
        }
        void DoOpenLanceTemplate(string lanceId)
        {
            TroopEditorViewService.Delete();
            LanceTemplateEditorController.CreateLayer(lanceId);
        }

        public void Update(bool refresh = true)
        {
            if (refresh) Refresh();
            else _equipmentService.RecalculateDefaultGroup();
        }
        public void OnSave()
        {
            var data = BuildTroopEditorData();
            var result = DynamicTroopsService.Instance.SaveCharacterFromData(data);
            if (!result.IsSuccess)
            {
                InformationManager.DisplayMessage(new InformationMessage(result.ErrorMessage!, new Color(1, 0, 0)));
                return;
            }
            MBTextManager.SetTextVariable("NAME", GetName());
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=lance_saved_full}{SAVED_PREFIX}: {NAME}")
                .SetTextVariable("SAVED_PREFIX", _savedPrefix).ToString()));
        }
        Dictionary<SkillObject, int> BuildSkillValues()
        {
            var values = new Dictionary<SkillObject, int>();
            foreach (var skill in TroopSkills.All)
                values[skill] = GetSkillValue(skill);
            return values;
        }
        public CharacterObject CreatePreviewCharacter()
        {
            CharacterObject preview = PreviewCharacter;
            var setNameMethod = AccessTools.Method("TaleWorlds.Core.BasicCharacterObject:SetName");
            setNameMethod.Invoke(preview, new object[] { new TextObject(GetName()) });
            preview.IsFemale = GetIsFemale();
            typeof(BasicCharacterObject).GetProperty("Culture")?.SetValue(preview, GetCulture());
            AccessTools.Property(typeof(BasicCharacterObject), "Level").SetValue(preview, GetLevel());
            typeof(CharacterObject).GetProperty("BodyPropertyRange")?.SetValue(preview, GetBodyProperty());
            MBCharacterSkills skills = MBObjectManager.Instance.CreateObject<MBCharacterSkills>(preview.StringId);
            foreach (var skill in TroopSkills.All)
                skills.Skills.SetPropertyValue(skill, GetSkillValue(skill));
            typeof(CharacterObject).GetField("DefaultCharacterSkills")?.SetValue(preview, skills);
            MBEquipmentRoster roster = GetRoster();
            typeof(BasicCharacterObject).GetField("_equipmentRoster")?.SetValue(preview, roster);
            preview.InitializeEquipmentsOnLoad(preview);
            return preview;
        }
        public static EquipmentIndex ToItemSlot(string equipment) => EquipmentService.ToItemSlot(equipment);
    }
}
