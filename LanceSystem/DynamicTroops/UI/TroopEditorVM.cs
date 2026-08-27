using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using LanceSystem.DynamicTroops.TroopCreation.Services;
namespace LanceSystem.DynamicTroops.TroopCreation
{
    public class TroopEditorVM : ViewModel
    {
        CharacterObject _character;
        readonly TroopEditorController _manager;
        string _name = "";
        string _tierText = "";
        string _totalSkillSum = "";
        string _itemSetText = "";
        MBBindingList<SkillRowVM> _skillRows = new();
        MBBindingList<ItemSlotVM> _weaponSlots = new();
        MBBindingList<ItemSlotVM> _armorSlots = new();
        MBBindingList<ItemSlotVM> _mountSlots = new();
        MBBindingList<UpgradeSlotVM> _upgradeSlots = new();
        MBBindingList<BindingListStringItem> _genderString = new();
        MBBindingList<BindingListStringItem> _cultureString = new();
        MBBindingList<BindingListStringItem> _faceString = new();
        MBBindingList<BindingListStringItem> _lanceNames = new();
        SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> _itemSetSelector;
        EncyclopediaUnitEquipmentSetSelectorItemVM _currentSelectedItemSet;
        readonly TextObject _itemSetTextObj = new("{=vggt7exj}Set {CURINDEX}/{COUNT}");
        CharacterViewModel _characterVM;
        [DataSourceProperty] public CharacterViewModel UnitCharacter { get => _characterVM; set { if (value != _characterVM) { _characterVM = value; OnPropertyChangedWithValue(value, "UnitCharacter"); } } }
        [DataSourceProperty] public string Name { get => _name; set { if (value != _name) { _name = value; OnPropertyChanged("Name"); } } }
        [DataSourceProperty] public string TierText { get => _tierText; set { if (value != _tierText) { _tierText = value; OnPropertyChangedWithValue(value, "TierText"); } } }
        [DataSourceProperty] public string TotalSkillSum { get => _totalSkillSum; set { if (value != _totalSkillSum) { _totalSkillSum = value; OnPropertyChangedWithValue(value, "TotalSkillSum"); } } }
        [DataSourceProperty] public string ItemSetText { get => _itemSetText; set { if (value != _itemSetText) { _itemSetText = value; OnPropertyChangedWithValue(value, "ItemSetText"); } } }
        [DataSourceProperty] public SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> ItemSetSelector { get => _itemSetSelector; set { if (value != _itemSetSelector) { _itemSetSelector = value; OnPropertyChangedWithValue(value, "ItemSetSelector"); } } }
        [DataSourceProperty] public EncyclopediaUnitEquipmentSetSelectorItemVM CurrentSelectedItemSet { get => _currentSelectedItemSet; set { if (value != _currentSelectedItemSet) { _currentSelectedItemSet = value; OnPropertyChangedWithValue(value, "CurrentSelectedItemSet"); } } }
        [DataSourceProperty] public MBBindingList<SkillRowVM> SkillRows { get => _skillRows; set { if (value != _skillRows) { _skillRows = value; OnPropertyChangedWithValue(value, "SkillRows"); } } }
        [DataSourceProperty] public MBBindingList<ItemSlotVM> WeaponSlots { get => _weaponSlots; set { if (value != _weaponSlots) { _weaponSlots = value; OnPropertyChangedWithValue(value, "WeaponSlots"); } } }
        [DataSourceProperty] public MBBindingList<ItemSlotVM> ArmorSlots { get => _armorSlots; set { if (value != _armorSlots) { _armorSlots = value; OnPropertyChangedWithValue(value, "ArmorSlots"); } } }
        [DataSourceProperty] public MBBindingList<ItemSlotVM> MountSlots { get => _mountSlots; set { if (value != _mountSlots) { _mountSlots = value; OnPropertyChangedWithValue(value, "MountSlots"); } } }
        [DataSourceProperty] public MBBindingList<UpgradeSlotVM> UpgradeSlots { get => _upgradeSlots; set { if (value != _upgradeSlots) { _upgradeSlots = value; OnPropertyChangedWithValue(value, "UpgradeSlots"); } } }
        [DataSourceProperty] public MBBindingList<BindingListStringItem> GenderString { get => _genderString; set { if (value != _genderString) { _genderString = value; OnPropertyChangedWithValue(value, "GenderString"); } } }
        [DataSourceProperty] public MBBindingList<BindingListStringItem> CultureString { get => _cultureString; set { if (value != _cultureString) { _cultureString = value; OnPropertyChangedWithValue(value, "CultureString"); } } }
        [DataSourceProperty] public MBBindingList<BindingListStringItem> FaceString { get => _faceString; set { if (value != _faceString) { _faceString = value; OnPropertyChangedWithValue(value, "FaceString"); } } }
        [DataSourceProperty] public MBBindingList<BindingListStringItem> LanceNames { get => _lanceNames; set { if (value != _lanceNames) { _lanceNames = value; OnPropertyChangedWithValue(value, "LanceNames"); } } }
        public TroopEditorVM(CharacterObject character, TroopEditorController manager)
        {
            _character = character;
            _manager = manager;
            UnitCharacter = new CharacterViewModel((CharacterViewModel.StanceTypes)1);
            UnitCharacter.FillFrom(_character, -1);
            BuildSkillRows();
            BuildUpgradeSlots();
            ItemSetSelector = new SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM>(0, OnItemSetChange);
            foreach (Equipment equipment in _character.BattleEquipments)
                if (!ItemSetSelector.ItemList.Any(x => x.EquipmentSet.IsEquipmentEqualTo(equipment)))
                    ItemSetSelector.AddItem(new EncyclopediaUnitEquipmentSetSelectorItemVM(equipment, ""));
            if (ItemSetSelector.ItemList.Count > 0) ItemSetSelector.SelectedIndex = 0;
            _itemSetTextObj.SetTextVariable("CURINDEX", ItemSetSelector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", ItemSetSelector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
            Name = _character.Name.ToString();
            BuildItemBindings();
            BuildAppearance();
            TierText = "Tier " + _character.Tier;
            TotalSkillSum = BuildTotalSkillSumText();
            LanceNames = new MBBindingList<BindingListStringItem> { new("Swadian Knights Lance - 12 men"), new("Vanguard Lance - 8 men"), new("Reserve Lance - 4 men") };
            FaceString = new MBBindingList<BindingListStringItem> { new("Face : Default") };
        }
        void BuildItemBindings()
        {
            int index = ItemSetSelector.SelectedIndex < 0 ? 0 : ItemSetSelector.SelectedIndex;
            Equipment[] equipments = _character.BattleEquipments.ToArray();
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
                new(DefaultSkills.OneHanded, _character.GetSkillValue(DefaultSkills.OneHanded), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.TwoHanded, _character.GetSkillValue(DefaultSkills.TwoHanded), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.Polearm, _character.GetSkillValue(DefaultSkills.Polearm), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.Bow, _character.GetSkillValue(DefaultSkills.Bow), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.Crossbow, _character.GetSkillValue(DefaultSkills.Crossbow), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.Throwing, _character.GetSkillValue(DefaultSkills.Throwing), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.Athletics, _character.GetSkillValue(DefaultSkills.Athletics), GetPointsLeft, UpdateSkill),
                new(DefaultSkills.Riding, _character.GetSkillValue(DefaultSkills.Riding), GetPointsLeft, UpdateSkill)
            };
        }
        void BuildUpgradeSlots()
        {
            UpgradeSlots = new MBBindingList<UpgradeSlotVM>();
            if (_character.UpgradeTargets == null) return;
            foreach (CharacterObject target in _character.UpgradeTargets) UpgradeSlots.Add(new UpgradeSlotVM(target, OnUpgradeLink, AddUpgrade, RemoveUpgrade));
        }
        void BuildAppearance()
        {
            GenderString = new MBBindingList<BindingListStringItem> { new("Gender : " + (_character.IsFemale ? "Female" : "Male")) };
            CultureString = new MBBindingList<BindingListStringItem> { new("Culture : " + (_character.Culture.Name).ToString()) };
        }
        string BuildTotalSkillSumText()
        {
            return "Total Skills: " + (GetAvailableSkillPoints() - GetPointsLeft()) + " / " + GetAvailableSkillPoints();
        }
        public void Close()
        {
            TroopEditorViewService.Delete();
        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            _character = MBObjectManager.Instance.GetObject<CharacterObject>(_character.StringId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>(_character.StringId);
            if (_character == null) return;
            Name = _character.Name.ToString();
            BuildSkillRows();
            BuildItemBindings();
            BuildAppearance();
            BuildUpgradeSlots();
            UnitCharacter.FillFrom(_character, -1);
            TierText = "Tier " + _character.Tier;
            TotalSkillSum = BuildTotalSkillSumText();
            FaceString.Clear();
            FaceString.Add(new BindingListStringItem("Face : Default"));
            _itemSetTextObj.SetTextVariable("CURINDEX", ItemSetSelector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", ItemSetSelector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
        }
        public int GetPointsLeft() => _manager.GetPointsLeft();
        public int GetAvailableSkillPoints() => _manager.GetAvailablePoints();
        public void UpdateSkill(SkillObject skill, int amount) => _manager.UpdateSkill(skill, amount);
        void SelectItem(string slotKey) => _manager.SelectItem(slotKey);
        void OnUpgradeLink(CharacterObject upgrade)
        {
            TroopEditorViewService.Delete();
            CharacterObject character = MBObjectManager.Instance.GetObject<CharacterObject>(upgrade.StringId) ?? Game.Current.ObjectManager.GetObject<CharacterObject>(upgrade.StringId);
            TroopEditorController next = new TroopEditorController(character);
            TroopEditorViewService.Create(next.Vm);
        }
        void RemoveUpgrade(CharacterObject upgrade)
        {
            _manager.RemoveUpgrade(upgrade);
            TroopEditorViewService.Delete();
            TroopEditorController next = new TroopEditorController(_character);
            TroopEditorViewService.Create(next.Vm);
        }
        void OnItemSetChange(SelectorVM<EncyclopediaUnitEquipmentSetSelectorItemVM> selector)
        {
            CurrentSelectedItemSet = selector.SelectedItem;
            UnitCharacter.SetEquipment(CurrentSelectedItemSet.EquipmentSet);
            _itemSetTextObj.SetTextVariable("CURINDEX", selector.SelectedIndex + 1);
            _itemSetTextObj.SetTextVariable("COUNT", selector.ItemList.Count);
            ItemSetText = _itemSetTextObj.ToString();
            RefreshValues();
        }
        public void Rename() => _manager.Rename();
        public void ChangeCulture() => _manager.ChangeCulture();
        public void ChangeGender() => _manager.ChangeGender();
        public void OpenTierInquiry()
        {
            List<InquiryElement> list = new();
            for (int i = 1; i <= 6; i++) list.Add(new InquiryElement(i, "Tier " + i, null));
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Tier", "", list, true, 1, 1, "Continue", null, args =>
            {
                InformationManager.HideInquiry();
                int tier = (int)args.First().Identifier;
                AccessTools.Field(typeof(CharacterObject), "_tier").SetValue(_character, tier);
                _manager.Refresh();
            }, null, "", false), false, false);
        }
        public void ChangeFace()
        {
            FaceString.Clear();
            FaceString.Add(new BindingListStringItem("Face : Randomized " + DateTime.Now.Second));
            InformationManager.DisplayMessage(new InformationMessage("Face editor opened (placeholder)"));
            _manager.Refresh();
        }
        public void AddUpgrade() => _manager.AddUpgrade();
        public void CopyTemplate() => _manager.CopyTemplate();
        public void PasteTemplate() => InformationManager.DisplayMessage(new InformationMessage("Paste template: placeholder - use Copy Template first"));
        public void SaveTroop()
        {
            _manager.Refresh();
            InformationManager.DisplayMessage(new InformationMessage("Troop saved: " + (_character.Name).ToString()));
        }
        public void SwitchTroop() => InformationManager.DisplayMessage(new InformationMessage("SwitchTroop pressed"));
        public void CreateNewTroop() => InformationManager.DisplayMessage(new InformationMessage("NewTroop pressed"));
        public void AddSet() => InformationManager.DisplayMessage(new InformationMessage("AddSet pressed"));
        public void RemoveSet() => InformationManager.DisplayMessage(new InformationMessage("RemoveSet pressed"));
    }
}
