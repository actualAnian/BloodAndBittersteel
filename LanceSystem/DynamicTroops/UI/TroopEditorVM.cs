using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using LanceSystem.DynamicTroops.UI.Services;
namespace LanceSystem.DynamicTroops.UI
{
    public class TroopEditorVM : ViewModel
    {
        CharacterObject _character;
        readonly TroopEditorController _manager;
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
        MBBindingList<BindingListStringItem> _lanceNames = new();
        SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> _itemSetSelector;
        EncyclopediaUnitEquipmentSetSelectorItemVM _currentSelectedItemSet;
        readonly TextObject _itemSetTextObj = new("{=vggt7exj}Set {CURINDEX}/{COUNT}");
        CharacterViewModel _characterVM;
        [DataSourceProperty] public CharacterViewModel UnitCharacter { get => _characterVM; set { if (value != _characterVM) { _characterVM = value; OnPropertyChangedWithValue(value, "UnitCharacter"); } } }
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
        [DataSourceProperty] public MBBindingList<BindingListStringItem> LanceNames { get => _lanceNames; set { if (value != _lanceNames) { _lanceNames = value; OnPropertyChangedWithValue(value, "LanceNames"); } } }
        public TroopEditorVM(CharacterObject character, TroopEditorController manager)
        {
            _character = character;
            _manager = manager;
            UnitCharacter = new CharacterViewModel((CharacterViewModel.StanceTypes)1);
            UnitCharacter.FillFrom(_character, -1);
            BuildSkillRows();
            BuildUpgradeSlots();
            BuildItemSetSelector();
            Name = _manager.GetName();
            BuildItemBindings();
            BuildAppearance();
            TierText = "Tier " + _manager.GetTier();
            TotalSkillSum = BuildTotalSkillSumText();
            LanceNames = new MBBindingList<BindingListStringItem> { new("Swadian Knights Lance - 12 men"), new("Vanguard Lance - 8 men"), new("Reserve Lance - 4 men") };
            FaceString = "Face : Default";
            FormationType = _manager.FillFormationString();
        }
        public void BuildItemSetSelector()
        {
            var previousIndex = ItemSetSelector == null ? 0 : ItemSetSelector.SelectedIndex;
            ItemSetSelector = new SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM>(0, OnItemSetChange);
            foreach (Equipment equipment in _manager.GetBattleEquipments())
                ItemSetSelector.AddItem(new EncyclopediaUnitEquipmentSetSelectorItemVM(equipment, ""));
            ItemSetSelector.SelectedIndex = previousIndex;
            _itemSetTextObj.SetTextVariable("CURINDEX", ItemSetSelector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", ItemSetSelector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
        }
        void BuildItemBindings()
        {
            int index = ItemSetSelector.SelectedIndex < 0 ? 0 : ItemSetSelector.SelectedIndex;
            Equipment[] equipments = _manager.GetBattleEquipments().ToArray();
            Equipment equipment = equipments[index];
            WeaponSlots = new MBBindingList<ItemSlotVM>
            {
                new("Wep0", "Weapon 1", equipment[EquipmentIndex.Weapon0].Item, SelectItem),
                new("Wep1", "Weapon 2", equipment[EquipmentIndex.Weapon1].Item, SelectItem),
                new("Wep2", "Weapon 3", equipment[EquipmentIndex.Weapon2].Item, SelectItem),
                new("Wep3", "Weapon 4", equipment[EquipmentIndex.Weapon3].Item, SelectItem)
            };
            ArmorSlots = new MBBindingList<ItemSlotVM>
            {
                new("Head", "Helmet", equipment[EquipmentIndex.Head].Item, SelectItem),
                new("Cape", "Cape", equipment[EquipmentIndex.Cape].Item, SelectItem),
                new("Body", "Chest", equipment[EquipmentIndex.Body].Item, SelectItem),
                new("Gloves", "Gloves", equipment[EquipmentIndex.Gloves].Item, SelectItem),
                new("Leg", "Boots", equipment[EquipmentIndex.Leg].Item, SelectItem)
            };
            MountSlots = new MBBindingList<ItemSlotVM>
            {
                new("Horse", "Mount", equipment[EquipmentIndex.Horse].Item, SelectItem),
                new("Harness", "Harness", equipment[EquipmentIndex.HorseHarness].Item, SelectItem)
            };
        }
        void BuildSkillRows()
        {
            SkillRows = new MBBindingList<SkillRowVM>
            {
                new(DefaultSkills.OneHanded, _manager.GetSkillValue(DefaultSkills.OneHanded), UpdateSkill),
                new(DefaultSkills.TwoHanded, _manager.GetSkillValue(DefaultSkills.TwoHanded), UpdateSkill),
                new(DefaultSkills.Polearm, _manager.GetSkillValue(DefaultSkills.Polearm), UpdateSkill),
                new(DefaultSkills.Bow, _manager.GetSkillValue(DefaultSkills.Bow), UpdateSkill),
                new(DefaultSkills.Crossbow, _manager.GetSkillValue(DefaultSkills.Crossbow), UpdateSkill),
                new(DefaultSkills.Throwing, _manager.GetSkillValue(DefaultSkills.Throwing), UpdateSkill),
                new(DefaultSkills.Athletics, _manager.GetSkillValue(DefaultSkills.Athletics), UpdateSkill),
                new(DefaultSkills.Riding, _manager.GetSkillValue(DefaultSkills.Riding), UpdateSkill)
            };
        }
        void BuildUpgradeSlots()
        {
            UpgradeSlots = new MBBindingList<UpgradeSlotVM>();
            var targets = _manager.GetUpgradeTargets();
            if (targets == null) return;
            foreach (CharacterObject target in targets) UpgradeSlots.Add(new UpgradeSlotVM(target, OnUpgradeLink, AddUpgrade, RemoveUpgrade));
        }
        void BuildAppearance()
        {
            GenderString = "Gender : " + (_manager.GetIsFemale() ? "Female" : "Male");
            CultureString = "Culture : " + _manager.GetCulture().Name.ToString();
        }
        string BuildTotalSkillSumText()
        {
            return "Total Skills: " + _manager.GetCurrentSkillSum() + " / " + _manager.GetSuggestedMaxSkillSum();
        }
        public void Close()
        {
            TroopEditorViewService.Delete();
        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            _character = _manager.CreatePreviewCharacter();
            if (_character == null) return;
            _character.Age = 50;
            BuildItemSetSelector();
            BuildItemBindings();
            BuildSkillRows();
            BuildAppearance();
            BuildUpgradeSlots();
            Name = _manager.GetName();
            UnitCharacter.FillFrom(_character, -1);
            if (CurrentSelectedItemSet != null)
                UnitCharacter.SetEquipment(CurrentSelectedItemSet.EquipmentSet);
            UnitCharacter.RefreshValues();
            TierText = "Tier " + _manager.GetTier();
            TotalSkillSum = BuildTotalSkillSumText();
            FormationType = _manager.FillFormationString();
        }
        public void MarkDirty() => _isDirty = true;
        public bool HasUnsavedChanges => _isDirty;
        public void UpdateSkill(SkillObject skill, int amount) { _manager.UpdateSkill(skill, amount); MarkDirty(); }
        void SelectItem(string slotKey) => _manager.SelectItem(slotKey);
        void OnUpgradeLink(CharacterObject upgrade) { _manager.SelectUpgradeCharacter(upgrade); MarkDirty(); }
        void RemoveUpgrade(CharacterObject upgrade)
        {
            _manager.RemoveUpgrade(upgrade);
            MarkDirty();
        }
        void OnItemSetChange(SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> selector)
        {
            CurrentSelectedItemSet = selector.SelectedItem;
            _itemSetTextObj.SetTextVariable("CURINDEX", selector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", selector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
            BuildItemBindings();
            UnitCharacter.FillFrom(_character, -1);
            UnitCharacter.SetEquipment(CurrentSelectedItemSet.EquipmentSet);
            UnitCharacter.RefreshValues();
        }
        public void Rename() { _manager.Rename(); MarkDirty(); }
        public void ChangeCulture() { _manager.ChangeCulture(); MarkDirty(); }
        public void ChangeGender() { _manager.ChangeGender(); MarkDirty(); }
        public void ChangeTier() { _manager.OpenTierInquiry(); MarkDirty(); }
        public void ChangeFace() { _manager.SelectAppearance(); MarkDirty(); }
        public void AddUpgrade() { _manager.AddUpgrade(); MarkDirty(); }
        public void CopyTemplate()
        {
            _manager.CopyTemplate();
        }
        public void PasteTemplate()
        {
            var template = EditorTemplateHolder.Instance.TroopPreviewCharacter;
            if (template == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("No troop template in clipboard. Copy a template first."));
                return;
            }
            if (HasUnsavedChanges)
            {
                InformationManager.ShowInquiry(new InquiryData("Unsaved Changes", "There are unsaved changes. Do you want to discard them and paste the template?", true, true, "Yes", "No", () => ApplyPaste(template), null, "", 0f, null, null, null), true, false);
                return;
            }
            ApplyPaste(template);
        }
        void ApplyPaste(CharacterObject template)
        {
            TroopEditorViewService.Delete();
            var controller = new TroopEditorController(template);
            TroopEditorViewService.Create(controller.Vm);
            InformationManager.DisplayMessage(new InformationMessage("Troop template pasted"));
        }
        public void SaveTroop()
        {
            _manager.OnSave();
            _isDirty = false;
        }
        public void SwitchTroop() => _manager.SwitchTroop();
        public void CreateNewTroop() => _manager.CreateNewTroop();
        public void AddSet() { _manager.AddSet(); MarkDirty(); }
        public void RemoveSet() { _manager.RemoveSet(); MarkDirty(); }
    }
}
