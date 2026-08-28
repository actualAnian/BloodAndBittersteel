using LanceSystem.DynamicTroops.UI.ItemSelection;
using LanceSystem.DynamicTroops.UI.ItemSelection.Filters;
using LanceSystem.DynamicTroops.UI.Services;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.DynamicTroops.UI
{
    public class TroopEditorController
    {
        public static bool DisableGearRestriction;
        public static bool DisableSkillTotalRestriction;
        public static bool DisableSkillCapRestriction;
        public CharacterObject Character { get; }
        public TroopEditorVM Vm { get; }
        readonly SkillService _skillService;
        readonly AppearanceService _appearanceService;
        readonly EquipmentService _equipmentService;
        readonly UpgradeService _upgradeService;
        public TroopEditorController(CharacterObject character)
        {
            Character = character;
            _skillService = new SkillService(character, Refresh);
            _appearanceService = new AppearanceService(character, Refresh);
            _equipmentService = new EquipmentService(character, Refresh);
            _upgradeService = new UpgradeService(character, Refresh);
            Vm = new TroopEditorVM(character, this);
        }
        public void Refresh()
        {
            _equipmentService.SetDefaultGroup();
            Vm.RefreshValues();
        }
        public void UpdateSkill(SkillObject skill, int amount) => _skillService.UpdateSkill(skill, amount);
        public int GetSkillCap() => _skillService.GetSkillCap();
        public int GetAvailablePoints() => _skillService.GetAvailablePoints();
        public int GetPointsLeft() => _skillService.GetPointsLeft();
        public void Rename() => _appearanceService.Rename();
        public void ChangeCulture() => _appearanceService.ChangeCulture();
        public void ChangeGender() => _appearanceService.ChangeGender();
        public void SelectAppearance()
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
                .Where(c => c.Occupation == Occupation.Soldier || c.Occupation == Occupation.Lord)
                .ToList();
            var controller = new ObjectSelectorController<CharacterObject>(allCharacters,
                (selectedCharacter) => { _appearanceService.UpdateAppearance(selectedCharacter.BodyPropertyRange); },
                (character, close) => new CharacterCardVM(character, (selectedCharacter) => { Vm.FaceString = new MBBindingList<BindingListStringItem>() { new(selectedCharacter.Name.ToString()) }; _appearanceService.UpdateAppearance(selectedCharacter.BodyPropertyRange); },
                close), FilterFactory.CreateCharacterFilters());
            controller.Open();

            _appearanceService.OpenAppearanceSelector();
        }
        public void SelectUpgradeCharacter(CharacterObject previouscharacter)
        {
            var allCharacters = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>()
            .Where(c => c.Occupation == Occupation.Soldier)
            .ToList();
            Action<CharacterObject> onSelect = (newCharacter =>
            {
                var slotToUpdate = Vm.UpgradeSlots.FirstOrDefault(slot => slot.Upgrade == previouscharacter);
                var slotWithNewCharacter = Vm.UpgradeSlots.Where(slot => slot.Upgrade == newCharacter);
                if (slotWithNewCharacter.Count() != 0) return;
                slotToUpdate.ChangeCharacter(newCharacter);
            }); 
            var controller = new ObjectSelectorController<CharacterObject>(allCharacters, onSelect, (character, close) => new CharacterCardVM(character, onSelect, close), FilterFactory.CreateCharacterFilters());
            controller.Open();
        }
        public void SelectItem(string slotKey) => _equipmentService.SelectItem(slotKey);
        public void FinalizeItem(EquipmentIndex index, ItemObject item) => _equipmentService.FinalizeItem(index, item);
        public void SetDefaultGroup() => _equipmentService.SetDefaultGroup();
        public void AddUpgrade() => _upgradeService.AddUpgrade();
        public void RemoveUpgrade(CharacterObject upgrade) => _upgradeService.RemoveUpgrade(upgrade);
        public void CopyTemplate() => _upgradeService.CopyTemplate();
        public void Update(bool refresh = true)
        {
            if (refresh) Refresh();
            else _equipmentService.SetDefaultGroup();
        }
        public static void CopyCharacter(CharacterObject original, CharacterObject target) => UpgradeService.CopyCharacter(original, target);
        public static EquipmentIndex ToItemSlot(string equipment) => EquipmentService.ToItemSlot(equipment);
    }
}
