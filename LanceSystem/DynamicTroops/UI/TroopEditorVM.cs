using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using LanceSystem.DynamicTroops.UI.Services;
using LanceSystem.UI;
using System;
namespace LanceSystem.DynamicTroops.UI
{
    public class TroopEditorVM : ViewModel
    {
        static readonly TextObject _titleText = new("{=lance_title}Troop Editor");
        static readonly TextObject _faceDefaultText = new("{=lance_face_default}Face : Default");
        static readonly TextObject _genderPrefixText = new("{=lance_gender_prefix}Gender");
        static readonly TextObject _genderFemaleText = new("{=lance_gender_female}Female");
        static readonly TextObject _genderMaleText = new("{=lance_gender_male}Male");
        static readonly TextObject _culturePrefixText = new("{=lance_culture_prefix}Culture");
        static readonly TextObject _totalSkillsPrefixText = new("{=lance_total_skills}Recommended Skills from tier");
        static readonly TextObject _skillsHeaderText = new("{=lance_skills_header}Skills");
        static readonly TextObject _armorHeaderText = new("{=lance_armor_header}Armor");
        static readonly TextObject _mountHeaderText = new("{=lance_mount_header}Mount");
        static readonly TextObject _weaponsHeaderText = new("{=lance_weapons_header}Weapons");
        static readonly TextObject _appearanceHeaderText = new("{=lance_appearance_header}Appearance");
        static readonly TextObject _upgradesHeaderText = new("{=lance_upgrades_header}Upgrades To");
        static readonly TextObject _lanceTemplatesHeaderText = new("{=lance_templates_header}Lance Templates");
        static readonly TextObject _addSetText = new("{=lance_add_set}Add Set");
        static readonly TextObject _removeSetText = new("{=lance_remove_set}Remove Set");
        static readonly TextObject _switchTroopText = new("{=lance_switch_troop}Switch Troop");
        static readonly TextObject _newTroopText = new("{=lance_new_troop}New Troop");
        static readonly TextObject _saveTroopText = new("{=lance_save_troop}Save Troop");
        static readonly TextObject _weapon1Label = new("{=lance_slot_weapon1}Weapon 1");
        static readonly TextObject _weapon2Label = new("{=lance_slot_weapon2}Weapon 2");
        static readonly TextObject _weapon3Label = new("{=lance_slot_weapon3}Weapon 3");
        static readonly TextObject _weapon4Label = new("{=lance_slot_weapon4}Weapon 4");
        static readonly TextObject _helmetLabel = new("{=lance_slot_helmet}Helmet");
        static readonly TextObject _capeLabel = new("{=lance_slot_cape}Cape");
        static readonly TextObject _chestLabel = new("{=lance_slot_chest}Chest");
        static readonly TextObject _glovesLabel = new("{=lance_slot_gloves}Gloves");
        static readonly TextObject _bootsLabel = new("{=lance_slot_boots}Boots");
        static readonly TextObject _mountLabel = new("{=lance_slot_mount}Mount");
        static readonly TextObject _harnessLabel = new("{=lance_slot_harness}Harness");
        static readonly TextObject _addUpgradeButtonText = new("{=lance_upgrade_button}Add troop upgrade");
        CharacterObject _character;
        readonly TroopEditorController _controller;
        string _name = "";
        string _tierText = "";
        string _totalSkillSum = "";
        string _itemSetText = "";
        string _genderString = "";
        string _cultureString = "";
        string _faceString = "";
        string _formationType;
        bool _isDirty;
        MBBindingList<SkillRowVM> _skillRows = new();
        MBBindingList<ItemSlotVM> _weaponSlots = new();
        MBBindingList<ItemSlotVM> _armorSlots = new();
        MBBindingList<ItemSlotVM> _mountSlots = new();
        MBBindingList<UpgradeSlotVM> _upgradeSlots = new();
        MBBindingList<LanceBannerItemVM> _lanceBanners = new();
        SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> _itemSetSelector;
        EncyclopediaUnitEquipmentSetSelectorItemVM _currentSelectedItemSet;
        readonly TextObject _itemSetTextObj = new("{=vggt7exj}Set {CURINDEX}/{COUNT}");
        CharacterViewModel _characterVM;
        [DataSourceProperty] public CharacterViewModel CharacterVM { get => _characterVM; set { if (value != _characterVM) { _characterVM = value; OnPropertyChangedWithValue(value, "UnitCharacter"); } } }
        [DataSourceProperty] public string Name { get => _name; set { if (value != _name) { _name = value; OnPropertyChanged("Name"); RefreshValues();} } }
        [DataSourceProperty] public string TierText { get => _tierText; set { if (value != _tierText) { _tierText = value; OnPropertyChangedWithValue(value, "TierText"); } } }
        [DataSourceProperty] public string FormationType { get => _formationType; set { if (value != _formationType) { _formationType = value; OnPropertyChangedWithValue(value, "FormationType"); } } }
        [DataSourceProperty] public string TotalSkillSum { get => _totalSkillSum; set { if (value != _totalSkillSum) { _totalSkillSum = value; OnPropertyChangedWithValue(value, "TotalSkillSum"); } } }
        [DataSourceProperty] public string ItemSetText { get => _itemSetText; set { if (value != _itemSetText) { _itemSetText = value; OnPropertyChangedWithValue(value, "ItemSetText"); } } }
        [DataSourceProperty] public SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> ItemSetSelector { get => _itemSetSelector; set { if (value != _itemSetSelector) { _itemSetSelector = value; OnPropertyChangedWithValue(value, "ItemSetSelector"); } } }
        [DataSourceProperty] public EncyclopediaUnitEquipmentSetSelectorItemVM CurrentSelectedItemSet { get => _currentSelectedItemSet; set { if (value != _currentSelectedItemSet) { _currentSelectedItemSet = value; OnPropertyChangedWithValue(value, "CurrentSelectedItemSet"); } } }
        [DataSourceProperty] public MBBindingList<SkillRowVM> SkillRows { get => _skillRows; set { if (value != _skillRows) { _skillRows = value; OnPropertyChangedWithValue(value, "SkillRows"); } } }
        [DataSourceProperty] public MBBindingList<ItemSlotVM> WeaponSlots { get => _weaponSlots; set { if (value != _weaponSlots) { _weaponSlots = value; OnPropertyChangedWithValue(value, "WeaponSlots"); } } }
        [DataSourceProperty] public MBBindingList<ItemSlotVM> ArmorSlots { get => _armorSlots; set { if (value != _armorSlots) { _armorSlots = value; OnPropertyChangedWithValue(value, "ArmorSlots"); } } }
        [DataSourceProperty] public MBBindingList<ItemSlotVM> MountSlots { get => _mountSlots; set { if (value != _mountSlots) { _mountSlots = value; OnPropertyChangedWithValue(value, "MountSlots"); } } }
        [DataSourceProperty] public MBBindingList<UpgradeSlotVM> UpgradeSlots { get => _upgradeSlots; set { if (value != _upgradeSlots) { _upgradeSlots = value; OnPropertyChangedWithValue(value, "UpgradeSlots"); } } }
        [DataSourceProperty] public string GenderString { get => _genderString; set { if (value != _genderString) { _genderString = value; OnPropertyChanged("GenderString"); } } }
        [DataSourceProperty] public string CultureString { get => _cultureString; set { if (value != _cultureString) { _cultureString = value; OnPropertyChanged("CultureString"); } } }
        [DataSourceProperty] public string FaceString { get => _faceString; set { if (value != _faceString) { _faceString = value; OnPropertyChanged("FaceString"); } } }
        [DataSourceProperty] public MBBindingList<LanceBannerItemVM> LanceBanners { get => _lanceBanners; set { if (value != _lanceBanners) { _lanceBanners = value; OnPropertyChangedWithValue(value, "LanceBanners"); } } }
        [DataSourceProperty] public string TitleBarText => _titleText.ToString();
        [DataSourceProperty] public string SkillsHeaderText => _skillsHeaderText.ToString();
        [DataSourceProperty] public string ArmorHeaderText => _armorHeaderText.ToString();
        [DataSourceProperty] public string MountHeaderText => _mountHeaderText.ToString();
        [DataSourceProperty] public string WeaponsHeaderText => _weaponsHeaderText.ToString();
        [DataSourceProperty] public string AppearanceHeaderText => _appearanceHeaderText.ToString();
        [DataSourceProperty] public string UpgradesHeaderText => _upgradesHeaderText.ToString();
        [DataSourceProperty] public string LanceTemplatesHeaderText => _lanceTemplatesHeaderText.ToString();
        [DataSourceProperty] public string EditButtonText => UITexts.Edit.ToString();
        [DataSourceProperty] public string AddButtonText => UITexts.Add.ToString();
        [DataSourceProperty] public string AddUpgradeButtonText => _addUpgradeButtonText.ToString();
        
        [DataSourceProperty] public string AddSetText => _addSetText.ToString();
        [DataSourceProperty] public string RemoveSetText => _removeSetText.ToString();
        [DataSourceProperty] public string SwitchTroopText => _switchTroopText.ToString();
        [DataSourceProperty] public string NewTroopText => _newTroopText.ToString();
        [DataSourceProperty] public string CopyTemplateText => UITexts.CopyTemplate.ToString();
        [DataSourceProperty] public string PasteTemplateText => UITexts.PasteTemplate.ToString();
        [DataSourceProperty] public string SaveTroopText => _saveTroopText.ToString();
        public TroopEditorVM(CharacterObject character, TroopEditorController manager)
        {
            _character = character;
            _controller = manager;
            CharacterVM = new CharacterViewModel((CharacterViewModel.StanceTypes)1);
            CharacterVM.FillFrom(_character, -1);
            BuildSkillRows();
            BuildUpgradeSlots();
            BuildItemSetSelector();
            Name = _controller.GetName();
            BuildItemBindings();
            BuildAppearance();
            TierText = BuildTierText();
            TotalSkillSum = BuildTotalSkillSumText();
            _lanceBanners = _controller.GetLanceBannersForTroop(OpenLanceTemplate);
            _faceString = _faceDefaultText.ToString();
            _formationType = _controller.FillFormationString();
        }
        public void BuildItemSetSelector()
        {
            var previousIndex = ItemSetSelector == null ? 0 : ItemSetSelector.SelectedIndex;
            ItemSetSelector = new SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM>(0, OnItemSetChange);
            foreach (Equipment equipment in _controller.GetBattleEquipments())
                ItemSetSelector.AddItem(new EncyclopediaUnitEquipmentSetSelectorItemVM(equipment, ""));
            ItemSetSelector.SelectedIndex = previousIndex;
            _itemSetTextObj.SetTextVariable("CURINDEX", ItemSetSelector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", ItemSetSelector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
        }
        void BuildItemBindings()
        {
            int index = ItemSetSelector.SelectedIndex < 0 ? 0 : ItemSetSelector.SelectedIndex;
            Equipment[] equipments = _controller.GetBattleEquipments().ToArray();
            Equipment equipment = equipments[index];
            WeaponSlots = new MBBindingList<ItemSlotVM>
            {
                new("Wep0", equipment[EquipmentIndex.Weapon0].Item != null ? "" : _weapon1Label.ToString(), equipment[EquipmentIndex.Weapon0].Item, SelectItem),
                new("Wep1", equipment[EquipmentIndex.Weapon1].Item != null ? "" : _weapon2Label.ToString(), equipment[EquipmentIndex.Weapon1].Item, SelectItem),
                new("Wep2", equipment[EquipmentIndex.Weapon2].Item != null ? "" : _weapon3Label.ToString(), equipment[EquipmentIndex.Weapon2].Item, SelectItem),
                new("Wep3", equipment[EquipmentIndex.Weapon3].Item != null ? "" : _weapon4Label.ToString(), equipment[EquipmentIndex.Weapon3].Item, SelectItem)
            };
            ArmorSlots = new MBBindingList<ItemSlotVM>
            {
                new("Head", equipment[EquipmentIndex.Head].Item != null ? "" : _helmetLabel.ToString(), equipment[EquipmentIndex.Head].Item, SelectItem),
                new("Cape", equipment[EquipmentIndex.Cape].Item != null ? "" : _capeLabel.ToString(), equipment[EquipmentIndex.Cape].Item, SelectItem),
                new("Body", equipment[EquipmentIndex.Body].Item != null ? "" : _chestLabel.ToString(), equipment[EquipmentIndex.Body].Item, SelectItem),
                new("Gloves", equipment[EquipmentIndex.Gloves].Item != null ? "" : _glovesLabel.ToString(), equipment[EquipmentIndex.Gloves].Item, SelectItem),
                new("Leg", equipment[EquipmentIndex.Leg].Item != null ? "" : _bootsLabel.ToString(), equipment[EquipmentIndex.Leg].Item, SelectItem)
            };
            MountSlots = new MBBindingList<ItemSlotVM>
            {
                new("Horse", equipment[EquipmentIndex.Horse].Item != null ? "" : _mountLabel.ToString(), equipment[EquipmentIndex.Horse].Item, SelectItem),
                new("Harness", equipment[EquipmentIndex.HorseHarness].Item != null ? "" : _harnessLabel.ToString(), equipment[EquipmentIndex.HorseHarness].Item, SelectItem)
            };
        }
        void BuildSkillRows()
        {
            SkillRows = new MBBindingList<SkillRowVM>();
            foreach (var skill in TroopSkills.All)
                SkillRows.Add(new(skill, _controller.GetSkillValue(skill), UpdateSkill));
        }
        void BuildUpgradeSlots()
        {
            UpgradeSlots = new MBBindingList<UpgradeSlotVM>();
            var targets = _controller.GetUpgradeTargets();
            if (targets == null) return;
            foreach (CharacterObject target in targets) UpgradeSlots.Add(new UpgradeSlotVM(target, OnUpgradeLink, AddUpgrade, RemoveUpgrade));
        }
        void BuildAppearance()
        {
            var genderText = _controller.GetIsFemale() ? _genderFemaleText : _genderMaleText;
            MBTextManager.SetTextVariable("GENDER", genderText);
            GenderString = new TextObject("{GENDER_PREFIX} : {GENDER}")
                .SetTextVariable("GENDER_PREFIX", _genderPrefixText).ToString();
            MBTextManager.SetTextVariable("CULTURE", _controller.GetCulture().Name);
            CultureString = new TextObject("{CULTURE_PREFIX} : {CULTURE}")
                .SetTextVariable("CULTURE_PREFIX", _culturePrefixText).ToString();
        }
        string BuildTierText()
        {
            MBTextManager.SetTextVariable("TIER", _controller.GetTier());
            return new TextObject("{TIER_PREFIX} {TIER}")
                .SetTextVariable("TIER_PREFIX", TroopEditorController._tierLabel).ToString();
        }
        string BuildTotalSkillSumText()
        {
            MBTextManager.SetTextVariable("CURRENT", _controller.GetCurrentSkillSum());
            MBTextManager.SetTextVariable("MAX", _controller.GetSuggestedMaxSkillSum());
            MBTextManager.SetTextVariable("NEWLINE", Environment.NewLine);
            return new TextObject("{=lance_total_skills_full}{TOTAL_SKILLS_PREFIX}: {CURRENT} / {MAX}. {NEWLINE} Shift click +10, Ctrl click +100")
                .SetTextVariable("TOTAL_SKILLS_PREFIX", _totalSkillsPrefixText).ToString();
        }
        public void Close()
        {
            TroopEditorViewService.Delete();
        }
        public void OpenLanceTemplate(string lanceId) => _controller.OpenLanceTemplate(lanceId);
        public override void RefreshValues()
        {
            base.RefreshValues();
            _character = _controller.CreatePreviewCharacter();
            if (_character == null) return;
            _character.Age = 50;
            BuildItemSetSelector();
            BuildItemBindings();
            BuildSkillRows();
            BuildAppearance();
            BuildUpgradeSlots();
            Name = _controller.GetName();
            CharacterVM.FillFrom(_character, -1);
            if (CurrentSelectedItemSet != null)
                CharacterVM.SetEquipment(CurrentSelectedItemSet.EquipmentSet);
            CharacterVM.RefreshValues();
            TierText = BuildTierText();
            TotalSkillSum = BuildTotalSkillSumText();
            FormationType = _controller.FillFormationString();
            LanceBanners = _controller.GetLanceBannersForTroop(OpenLanceTemplate);
            FaceString = _faceDefaultText.ToString();
        }
        public void MarkDirty() => _isDirty = true;
        public bool HasUnsavedChanges => _isDirty;
        public void UpdateSkill(SkillObject skill, int amount) { _controller.UpdateSkill(skill, amount); MarkDirty(); }
        void SelectItem(string slotKey) => _controller.SelectItem(slotKey);
        void OnUpgradeLink(CharacterObject upgrade) { _controller.SelectUpgradeCharacter(upgrade); MarkDirty(); }
        void RemoveUpgrade(CharacterObject upgrade)
        {
            _controller.RemoveUpgrade(upgrade);
            MarkDirty();
        }
        void OnItemSetChange(SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> selector)
        {
            CurrentSelectedItemSet = selector.SelectedItem;
            _itemSetTextObj.SetTextVariable("CURINDEX", selector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", selector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
            BuildItemBindings();
            CharacterVM.FillFrom(_character, -1);
            CharacterVM.SetEquipment(CurrentSelectedItemSet.EquipmentSet);
            CharacterVM.RefreshValues();
        }
        public void Rename() { _controller.Rename(); MarkDirty(); }
        public void ChangeCulture() { _controller.ChangeCulture(); MarkDirty(); }
        public void ChangeGender() { _controller.ChangeGender(); MarkDirty(); }
        public void ChangeTier() { _controller.OpenTierInquiry(); MarkDirty(); }
        public void ChangeFace() { _controller.SelectAppearance(); MarkDirty(); }
        public void AddUpgrade() { _controller.AddUpgrade(); MarkDirty(); }
        public void CopyTemplate() => _controller.CopyTemplate();
        public void PasteTemplate() => _controller.PasteTemplate();
        public void SaveTroop()
        {
            _controller.OnSave();
            _isDirty = false;
        }
        public void SwitchTroop() => _controller.SwitchTroop();
        public void CreateNewTroop() => _controller.CreateNewTroop();
        public void AddSet() { _controller.AddSet(); MarkDirty(); }
        public void RemoveSet() { _controller.RemoveSet(); MarkDirty(); }
    }
}
